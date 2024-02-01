using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInkBlast : MonoBehaviour
{
    private float damage = 28f;

    private ObjectPooler pooler;

    [SerializeField] private SpriteRenderer frameRenderer;
    [SerializeField] private SpriteRenderer fillRenderer;
    [SerializeField] private ParticleSystem inkParticle;

    [SerializeField] private LayerMask targetLayer = 8;
    [SerializeField] private Collider2D areaCollider;

    private float timeForFill = 1.25f;

    public void Active(float damage, ObjectPooler pooler) {
        gameObject.SetActive(true);
        this.damage = damage;
        this.pooler = pooler;
        
        StartCoroutine(FillCoroutine());
    }
    
    private void OnEnable() {
        fillRenderer.size = new Vector2(0, fillRenderer.size.y);
    }

    private IEnumerator FillCoroutine() {
        float step = 1f / timeForFill;
        float offset = 0;
        while(offset < 1) {
            offset += step * Time.deltaTime;
            fillRenderer.size = new Vector2(offset, fillRenderer.size.y);
            yield return null;
        }

        AttackArea();
        frameRenderer.enabled = false;
        fillRenderer.enabled = false;

        yield return new WaitForSeconds(5f);
        pooler.InPool(gameObject);
    }

    private void AttackArea() {
        List<Collider2D> inners = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D() {
            layerMask = targetLayer.value,
            useTriggers = true,
            useLayerMask = true,
        };
        Physics2D.OverlapCollider(areaCollider, filter, inners);
        foreach(var inner in inners) {
            if(inner != null && inner.TryGetComponent(out Character character)) {
                character.TakeDamage(damage);
            }
        }
        inkParticle.Play();
    }
}