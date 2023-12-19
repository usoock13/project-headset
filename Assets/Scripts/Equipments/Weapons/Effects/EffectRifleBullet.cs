using System.Collections.Generic;
using UnityEngine;

public class EffectRifleBullet : EffectProjectile {
    public WeaponRifle originWeapon;
    private float Damage => originWeapon.Damage;
    private float flyingSpeed = 36f;
    private float hittingDelay = 0.4f;
    private int MaxHitCount => originWeapon.MaxHitCount;
    private int hitCount = 0;

    private const float FLYING_TIME = 3;

    private List<GameObject> hitMonsters = new List<GameObject>();

    [SerializeField] LayerMask targetLayer = 1<<8;
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
        hitCount = 0;
        hitMonsters.Clear();
        trailRenderer.Clear();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == targetLayer.value
        && !hitMonsters.Contains(other.gameObject)) {
            if(other.TryGetComponent(out Monster target)) {
                target.TakeDamage(Damage);
                target.TakeAttackDelay(hittingDelay);
                target.TakeForce(transform.up * 1f, hittingDelay);
                hitMonsters.Add(other.gameObject);
                GameManager.instance.Character.OnAttackMonster(target);
                
                if(++hitCount >= MaxHitCount)
                    Disappear();
            }
        }
    }
    protected override void Disappear() {
        base.Disappear();
        originWeapon.EffectPooler.InPool(this.gameObject);
    }
}