using System;
using UnityEngine;

public class EffectSpikeTrap : MonoBehaviour {
    [SerializeField] private WeaponSpikeTrap originWeapon;

    [SerializeField] private LayerMask targetLayer = 1<<8;
    private float damage = 0;
    private float HittingDelay => originWeapon.HittingDelay;
    private float lifetime = 0;
    private float duration = 2;

    private void OnEnable() {
        lifetime = 0;
        this.damage = originWeapon.Damage;
        this.duration = originWeapon.Duration;
    }
    private void Update() {
        lifetime += Time.deltaTime;
        if(lifetime >= duration)
            Disapear();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == targetLayer.value) {
            if(other.TryGetComponent(out Monster monster)) {
                monster.TakeDamage(damage);
                monster.TakeAttackDelay(HittingDelay);
            }
            GameManager.instance.Character.OnAttackMonster(monster);
            Disapear();
        }
    }
    private void Disapear() {
        originWeapon.EffectPooler.InPool(this.gameObject);
    }
}