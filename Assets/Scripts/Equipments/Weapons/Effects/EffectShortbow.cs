using System;
using System.Collections;
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
    [SerializeField] TrailRenderer trailRenderer;

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
        trailRenderer.Clear();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive
        && 1<<other.gameObject.layer == targetLayer.value) {
            if(other.TryGetComponent(out Monster target)) {
                target.TakeDamage(Damage);
                target.TakeAttackDelay(hittingDelay);
                target.TakeForce(transform.up * .2f, hittingDelay);
                isActive = false;
                Disappear();
                GameManager.instance.Character.OnAttackMonster(target);
            }
        }
    }
}