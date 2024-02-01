using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectEraser : MonoBehaviour
{
    private bool isActive = false;

    private float dps = 98f;
    private readonly float damageInterval = 0.15f;
    private float duration = 5f;

    private ObjectPooler pooler;

    private Transform target;

    [SerializeField] private ParticleSystem summoningParticle;
    [SerializeField] private ParticleSystem rayParticle;
    [SerializeField] private ParticleSystem splashParticle;

    [SerializeField] private LayerMask targetLayer = 8;
    [SerializeField] private Collider2D areaCollider;

    private float timeForSummon = 1.25f;

    private void Update() {
        if(isActive && target != null)
            transform.Translate((target.position - transform.position).normalized * 1.5f * Time.deltaTime);
    }

    public void Active(float dps, ObjectPooler pooler, Transform target) {
        gameObject.SetActive(true);
        this.dps = dps;
        this.pooler = pooler;
        this.target = target;

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        summoningParticle.Play();
        yield return new WaitForSeconds(timeForSummon);

        splashParticle.Play();
        rayParticle.Play();

        var inners = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D() {
            layerMask = targetLayer.value,
            useTriggers = true,
            useLayerMask = true,
        };

        isActive = true;
        float time = 0f;

        while(time < duration) {
            Physics2D.OverlapCollider(areaCollider, filter, inners);
            foreach(var inner in inners) {
                if(inner != null && inner.TryGetComponent(out Character character)) {
                    character.TakeDamage(dps * damageInterval);
                }
            }
            time += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
        isActive = false;
        pooler.InPool(gameObject);
    }
}