using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectXXXLStrike : MonoBehaviour {
    [SerializeField] private AWeaponXXXLCalibur originWeapon;

    [SerializeField] private ParticleSystem secondParticle;

    [SerializeField] private LayerMask targetLayer;

    private float SecondDamage => originWeapon.SecondDamage;

    private float sAttackDelay = 1.35f;

    private float sForceScalar = 1f;

    [SerializeField] Collider2D secondAttackArea;

    
    public void Active() {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        secondParticle.Play();
        yield return new WaitForSeconds(0.3f);
        SecondAttack();
        yield return new WaitForSeconds(2f);
        originWeapon.SecondEffectPooler.InPool(this.gameObject);
    }

    private void SecondAttack() {
        var inners = new List<Collider2D>();
        var filter = new ContactFilter2D() {
            layerMask = targetLayer
        };
        secondAttackArea.OverlapCollider(filter, inners);
        for(int i=0; i<inners.Count; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(SecondDamage);
                monster.TakeAttackDelay(sAttackDelay);
                monster.TakeForce((monster.transform.position - transform.position).normalized * sForceScalar);
                GameManager.instance.Character.OnAttackMonster(monster);
            }
        }
    }
}