using System.Collections;
using UnityEngine;

public class EffectAwFireArrow : EffectProjectile {
    [SerializeField] private AWeaponBlaze originWeapon;

    [SerializeField] private float flyingSpeed = 18f;
    private float currentSpeed = 0;

    protected override void OnEnable() {
        base.OnEnable();
        currentSpeed = flyingSpeed;
    }
    protected override void Update() {
        base.Update();
        currentSpeed -= Time.deltaTime * 18f / 0.6f;
        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Monster"
        && other.TryGetComponent(out Monster monster)) {
            monster.TakeDamage(originWeapon.ArrowDamage);
            monster.TakeForce(transform.up * 0.5f, 0.5f);
            monster.TakeAttackDelay(.4f);
            Disappear();
        }
    }

    protected override void Disappear() {
        base.Disappear();
        originWeapon.ArrowPooler.InPool(this.gameObject);
    }
}