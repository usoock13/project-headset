using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBranchOfInkStorm : MonoBehaviour {
    [SerializeField] private EffectInkStorm origin;
    [SerializeField] private LayerMask targetLayer = 1<<3;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem chargingParticle;
    [SerializeField] private Collider2D areaCollider;

    private bool isActive = false;

    public void Active(float delay) {
        isActive = true;
        spriteRenderer.enabled = false;
        StartCoroutine(AttackCoroutine(delay));
    }
    public void Inactive() {
        isActive = false;
        spriteRenderer.enabled = false;
    }

    private IEnumerator AttackCoroutine(float delay) {
        chargingParticle.Play();
        yield return new WaitForSeconds(delay);

        spriteRenderer.enabled = true;
        List<Collider2D> inners = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D() {
            useTriggers = true,
            useLayerMask = true,
            layerMask = targetLayer.value,
        };

        while(isActive) {
            Physics2D.OverlapCollider(areaCollider, filter, inners);
            for(int i=0; i<inners.Count; i++) {
                if(inners[i].TryGetComponent(out Character character)) {
                    character.TakeDamage(origin.DPS * origin.Interval);
                }
            }
            yield return new WaitForSeconds(origin.Interval);
        }
    }
}
