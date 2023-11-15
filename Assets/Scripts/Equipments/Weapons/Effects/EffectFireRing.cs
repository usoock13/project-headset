using System;
using UnityEngine;

public class EffectFireRing : MonoBehaviour {
    private bool isActive = false;
    private float explosionRadius = 0.5f;
    private float attackForce = 1f;
    private float hittingDelay = 0.6f;
    [SerializeField] private WeaponFireRing originWeapon;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private ParticleSystem trailParticle;

    public void Active() {
        particle.Play();
        isActive = true;
    }
    private void Inactive() {
        particle.Stop();
        isActive = false;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive
        && 1<<other.gameObject.layer == targetLayer.value) {
            if(other.TryGetComponent(out Monster monster)) {
                Collider2D[] inners = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                for(int i=0; i<inners.Length; i++) {
                    if(inners[i].TryGetComponent(out Monster target)) {
                        Vector2 forceDir = (target.transform.position - transform.position).normalized;
                        target.TakeDamage(originWeapon.Damage);
                        target.TakeHittingDelay(hittingDelay);
                        target.TakeForce(forceDir * attackForce);
                        explosionParticle.Play();
                        Inactive();
                    }
                }
            }
        }
    }
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 1, 0, 1);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    #endif
}