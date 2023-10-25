using System.Collections.Generic;
using UnityEngine;

public class EffectMaliciousFlask : EffectProjectile {
    public WeaponMaliciousFlask originWeapon;
    private float Damage => originWeapon.Damage;
    private float flyingSpeed = 45f;
    private float currentSpeed = 0;
    private float hittingDelay = 0.4f;
    [SerializeField] LayerMask targetLayer = 8;

    private void Start() {
        currentSpeed = flyingSpeed;
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    protected override void Update() {
        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
        currentSpeed = Mathf.Lerp(flyingSpeed, 0, lifetime/flyingTime);
        lifetime += Time.deltaTime * 5;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == targetLayer.value) {
            Monster target;
            if(other.TryGetComponent<Monster>(out target)) {
                target.TakeDamage(Damage);
                target.TakeHittingDelay(hittingDelay);
                target.TakeForce(transform.up * 1f, hittingDelay);
                Disapear();
            }
        }
    }
}