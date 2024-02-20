using UnityEngine;

public class EffectStormArrow : EffectProjectile {
    private bool isActive = false;
    [SerializeField] private SkillRanger origin;
    [SerializeField] private float flyingSpeed = 15f;

    protected override void Update() {
        base.Update();
        transform.Translate(Vector2.up * flyingSpeed * Time.deltaTime);
        transform.Rotate(Vector3.forward, 360f / (flyingTime * 0.8f) * Time.deltaTime);
    }

    protected override void OnEnable() {
        base.OnEnable();
        isActive = true;
    }

    protected override void Disappear() {
        if(isActive) {
            origin.ArrowPooler.InPool(gameObject);
            isActive = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if((1<<other.gameObject.layer | origin.targetLayer.value) > 0 && other.TryGetComponent(out Monster monster)) {
            monster.TakeDamage(origin.Damage);
            monster.TakeStagger(0.12f);
            monster.TakeForce(transform.up * 0.12f);
            Disappear();
        }
    }
}