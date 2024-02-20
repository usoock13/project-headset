using System;
using System.Collections;
using UnityEngine;

public class EffectSpikeTrap : MonoBehaviour {
    [SerializeField] private WeaponSpikeTrap originWeapon;

    [SerializeField] private LayerMask targetLayer = 1<<8;
    [SerializeField] private float radius = 0.5f;
    private float damagePerSecond = 0;
    private float HittingDelay => originWeapon.HittingDelay;
    private float lifetime = 0;
    private float duration = 2;
    private readonly float damageInterval = 0.25f;

    private void OnEnable() {
        lifetime = 0;
        this.damagePerSecond = originWeapon.DamagePerSecond;
        this.duration = originWeapon.Duration;
        this.radius = originWeapon.TrapScale;
        transform.localScale = Vector3.one * originWeapon.TrapScale;
        StartCoroutine(DamageCoroutine());
    }

    private IEnumerator DamageCoroutine() {
        while(lifetime < duration) {
            DamageArea();
            lifetime += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
        Disappear();
    }
    private void DamageArea() {
        var inners = Physics2D.OverlapCircleAll(transform.position, radius);
        for(int i=0; i<inners.Length; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(damagePerSecond * damageInterval);
                monster.TakeStagger(HittingDelay);
                GameManager.instance.Character.OnAttackMonster(monster);
            }
        }
    }
    private void Disappear() {
        originWeapon.EffectPooler.InPool(this.gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}