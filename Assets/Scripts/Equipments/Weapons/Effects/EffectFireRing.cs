using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EffectFireRing : MonoBehaviour {
    [SerializeField] private WeaponFireRing originWeapon;
    private bool isActive = false;

    private float explosionRadius = 0.5f;
    private float attackForce = 1.2f;
    private float hittingDelay = 0.6f;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private ParticleSystem trailParticle;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;

    private float reactiveCountdown = 0;

    public void Active() {
        particle.gameObject.SetActive(true);
        trailParticle.gameObject.SetActive(true);
        particle.Play();
        trailParticle.Play();
        isActive = true;
    }
    private void Inactive() {
        isActive = false;
        particle.Stop();
        trailParticle.Stop();
        particle.gameObject.SetActive(false);
        trailParticle.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive
        && 1<<other.gameObject.layer == targetLayer.value) {
            if(other.TryGetComponent(out Monster monster)) {
                Collider2D[] inners = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                for(int i=0; i<inners.Length; i++) {
                    if(inners[i].TryGetComponent(out Monster target)) {
                        Vector3 characterPoint = GameManager.instance.Character.transform.position;
                        Vector2 forceDir = (target.transform.position - characterPoint).normalized;
                        target.TakeDamage(originWeapon.Damage);
                        target.TakeAttackDelay(hittingDelay);
                        target.TakeForce(forceDir * attackForce, hittingDelay);
                        audioSource.PlayOneShot(explosionSound);
                        explosionParticle.Play();
                        Inactive();
                    }
                }
            }
        }
    }
    private void Update() {
        if(!isActive) {
            reactiveCountdown += Time.deltaTime;
            if(reactiveCountdown >= originWeapon.ReactiveInterval) {
                reactiveCountdown = 0;
                Active();
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