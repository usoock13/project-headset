using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectXXXLSlash : MonoBehaviour {
    [SerializeField] private AWeaponXXXLCalibur originWeapon;

    [SerializeField] private ParticleSystem firstParticle;

    [SerializeField] private LayerMask targetLayer;

    private float FirstDamage => originWeapon.Damage;

    private float fAttackDelay = 0.6f;

    private float fForceScalar = 0.6f;

    [SerializeField] Collider2D firstAttackArea;

    
    public void Active() {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        firstParticle.Play();
        FirstAttack();
        yield return new WaitForSeconds(2f);
        originWeapon.FirstEffectPooler.InPool(this.gameObject);
    }

    private void FirstAttack() {
        var inners = new List<Collider2D>();
        var filter = new ContactFilter2D() {
            layerMask = targetLayer,
            useLayerMask = true,
        };
        firstAttackArea.OverlapCollider(filter, inners);
        for(int i=0; i<inners.Count; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(FirstDamage);
                monster.TakeAttackDelay(fAttackDelay);
                monster.TakeForce((monster.transform.position - transform.position).normalized * fForceScalar);
                GameManager.instance.Character.OnAttackMonster(monster);
            }
        }
    }
}