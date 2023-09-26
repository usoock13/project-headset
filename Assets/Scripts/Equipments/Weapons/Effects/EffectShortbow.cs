using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectShortbow : EffectProjectile {
    private bool isActive = false;
    public WeaponShortbow originWeapon;
    private float Damage => originWeapon.Damage;
    private float flyingSpeed = 12f;
    private float hittingDelay {
        get => originWeapon.HittingDelay;
    }
    [SerializeField] LayerMask targetLayer = 8;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    protected override void Update() {
        base.Update();
        transform.Translate(Vector2.up * flyingSpeed * Time.deltaTime);
    }
    protected override void OnEnable() {
        base.OnEnable();
        isActive = true;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive
        && 1<<other.gameObject.layer == targetLayer.value) {
            Monster target;
            if(other.TryGetComponent<Monster>(out target)) {
                target.TakeDamage(Damage);
                target.TakeHittingDelay(hittingDelay);
                target.TakeForce(transform.up * .2f, hittingDelay);
                isActive = false;
                Disapear();
            }
        }
    }
}