using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAwFireball : MonoBehaviour {
    [SerializeField] protected AWeaponBlaze originWeapon;

    private bool isActive = false;

    private readonly float explosionRadius = 1.0f;
    protected readonly float attackForce = 1.2f;
    protected readonly float hittingDelay = 0.6f;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ParticleSystem trailParticle;
    [SerializeField] private ParticleSystem explosionParticle;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;

    private float reactiveCountdown = 0;
    
    private void Update() {
        if(!isActive) {
            reactiveCountdown += Time.deltaTime;
            if(reactiveCountdown >= originWeapon.ReactiveInterval) {
                reactiveCountdown = 0;
                Active();
            }
        }
    }
    
    public void Active() {
        particle.gameObject.SetActive(true);
        particle.Play();
        trailParticle.Play();
        isActive = true;
    }

    private void Inactive() {
        isActive = false;
        particle.Stop();
        trailParticle.Stop();
        particle.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive
        && (1<<other.gameObject.layer & targetLayer.value) > 0) {
            Collider2D[] inners = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayer);
            List<Monster> monsters = new List<Monster>();
            for(int i=0; i<inners.Length; i++) {
                if(inners[i].TryGetComponent(out Monster monster))
                    monsters.Add(monster);
            }
            this.AttackMonsters(monsters.ToArray());
            explosionParticle.Play();
            audioSource.PlayOneShot(explosionSound);
            Inactive();
        }
    }

    protected void AttackMonsters(Monster[] monsters) {
        for(int i=0; i<monsters.Length; i++) {
            Vector3 characterPoint = GameManager.instance.Character.transform.position;
            Vector2 forceDir = (monsters[i].transform.position - characterPoint).normalized;
            monsters[i].TakeDamage(originWeapon.Damage);
            monsters[i].TakeStagger(hittingDelay);
            monsters[i].TakeForce(forceDir * attackForce, hittingDelay);
            GameManager.instance.Character.OnAttackMonster(monsters[i]);

            StartCoroutine(ShootFireArrows());
        }
    }

    private IEnumerator ShootFireArrows() {
        for(int i=0; i<originWeapon.NumberOfArrow; i++) {
            var arrow = originWeapon.ArrowPooler.OutPool(transform.position, Quaternion.identity);
            arrow.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0, 360f));
            yield return new WaitForSeconds(0.02f);
        }
    }
}