using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTentacle : MonoBehaviour {
    private bool isActive = false;

    [SerializeField] private WeaponTentacleBag originWeapon;
    private float Damage => originWeapon.Damage;

    private int MaxAttackCount => originWeapon.AttackCount;
    private int attackCount = 0;
    private float interval => 0.08f;
    private float countdown = 0f;
    private float attackForceScalar = 0.1f;

    private List<Monster> inners = new List<Monster>();
    [SerializeField] private ParticleSystem effectParticle;

    public void Activate() {
        isActive = true;
        effectParticle.Play();
    }

    private void OnEnable() {
        attackCount = 0;
    }
    
    private void OnDisable() {
        inners.Clear();
    }

    private void Update() {
        if(!isActive)
            return;
            
        if(attackCount < MaxAttackCount) {
            countdown -= Time.deltaTime;
            if(countdown <= 0) {
                countdown += interval;
                AttackArea();
                attackCount ++;
            }
        } else {
            StartCoroutine(InPoolCoroutine());
        }
    }

    private IEnumerator InPoolCoroutine() {
        isActive = false;
        yield return new WaitForSeconds(3f);
        originWeapon.EffectPooler.InPool(this.gameObject);
    }

    private void AttackArea() {
        for(int i=0; i<inners.Count; i++) {
            inners[i].TakeDamage(Damage);
            inners[i].TakeAttackDelay(interval * 1.1f);
            inners[i].TakeForce(transform.up * attackForceScalar);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent(out Monster monster))
            inners.Add(monster);
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.TryGetComponent(out Monster monster))
            inners.Remove(monster);
    }
}