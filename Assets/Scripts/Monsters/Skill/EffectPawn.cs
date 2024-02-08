using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPawn : MonoBehaviour
{
    private float damage = 40f;

    private ObjectPooler pooler;

    [SerializeField] private SpriteRenderer frameRenderer;
    [SerializeField] private SpriteRenderer fillRenderer;

    [SerializeField] private LayerMask targetLayer = 8;
    [SerializeField] private Collider2D areaCollider;
    [SerializeField] new private Rigidbody2D rigidbody;

    /*  */
    [SerializeField] new private SpriteRenderer renderer;
    /*  */

    private float timeForFill = 1.5f;

    public void Active(float damage, ObjectPooler pooler) {
        gameObject.SetActive(true);
        this.damage = damage;
        this.pooler = pooler;
        
        StartCoroutine(FillCoroutine());
    }
    
    private void OnEnable() {
        renderer.enabled = false;
        rigidbody.simulated = false;
        frameRenderer.enabled = true;
        fillRenderer.enabled = true;
        fillRenderer.size = new Vector2(0, 0);
    }

    private IEnumerator FillCoroutine() {
        float step = 1f / timeForFill;
        float offset = 0;
        while(offset < 1) {
            offset += step * Time.deltaTime;
            fillRenderer.size = new Vector2(offset, offset);
            yield return null;
        }
        Summon();
        frameRenderer.enabled = false;
        fillRenderer.enabled = false;

        yield return new WaitForSeconds(8f);
        Disappear();
    }

    private void Summon() {
        List<Collider2D> inners = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D() {
            layerMask = targetLayer.value,
            useTriggers = true,
            useLayerMask = true,
        };
        Physics2D.OverlapCollider(areaCollider, filter, inners);
        if(inners.Count > 0) {
            foreach(var inner in inners) {
                if(inner != null && inner.TryGetComponent(out Character character)) {
                    character.TakeDamage(damage);
                }
            }
        } else {
            /*  */
            rigidbody.simulated = true;
            renderer.enabled = true;
            /*  */
        }
    }

    private void Disappear() {
        pooler.InPool(gameObject);
    }
}