using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectItemBomb : MonoBehaviour {
    [SerializeField] private SkillAlchemist origin;

    [SerializeField] private float delay = 0.5f;
    [SerializeField] private float damage = 0;

    [SerializeField] private LayerMask targetLayer = 1<<8;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteMask spriteMask;
    [SerializeField] private Animation anim;

    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private Collider2D explosionCollider;
    
    public void Active(Sprite sprite, float damage) {
        anim.Play();
        spriteRenderer.enabled = true;
        spriteMask.enabled = true;

        this.damage = damage;
        spriteRenderer.sprite = sprite;
        spriteMask.sprite = sprite;
        StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine() {
        yield return new WaitForSeconds(delay);
        Explosion();
        yield return new WaitForSeconds(3f);
        origin.BombPooler.InPool(this.gameObject);
    }
    private void Explosion() {
        spriteRenderer.enabled = false;
        spriteMask.enabled = false;

        var inners = new List<Collider2D>();
        var filter = new ContactFilter2D() {
            useTriggers = true,
            useLayerMask = true,
            layerMask = targetLayer,
        };

        Physics2D.OverlapCollider(explosionCollider, filter, inners);
        for(int i=0; i<inners.Count; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(damage);
                monster.TakeStagger(1.0f);
                monster.TakeForce((monster.transform.position - transform.position).normalized * 1.0f, 1.0f);
            }
        }

        explosionParticle.Play();
    }
}