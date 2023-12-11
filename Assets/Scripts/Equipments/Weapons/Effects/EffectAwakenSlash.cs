using System.Collections;
using UnityEngine;

public class EffectAwakenSlash : MonoBehaviour {
    [SerializeField] private AwakenWeaponSword originWeapon;

    [SerializeField] private ParticleSystem firstParticle;
    [SerializeField] private ParticleSystem secondParticle;

    private float FirstDamage => originWeapon.Damage;

    [SerializeField] Collider2D firstAttackArea;
    [SerializeField] Collider2D secondAttackArea;


    private float termOfAttacks = 0.5f;

    
    public void Active() {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        FirstAttack();
        yield return new WaitForSeconds(termOfAttacks);
        SecondAttack();
    }

    private void FirstAttack() {
        
    }

    private void SecondAttack() {
        
    }
}