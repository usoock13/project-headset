using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectFlyingAxe : EffectProjectile {
    public WeaponAxe originWeapon;
    private float Damage => originWeapon.Damage;
    private float Scale => originWeapon.ProjectileScale;
    [SerializeField] private float flyingSpeed = 22f;
    private float hittingDelay = 0.8f;
    private List<GameObject> hitMonsters = new List<GameObject>();
    [SerializeField] LayerMask targetLayer = 1<<8;

    private float DefaultColliderRadius => circleCollider.radius;
    [SerializeField] private SpriteRenderer flyingAxeSprite;
    [SerializeField] private CircleCollider2D circleCollider;
    public Action onDisable;
    private int currentLevel = 0;

    private Vector2 velocity;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
        // circleCollider = GetComponent<CircleCollider2D>(); // Please input on inspector window.
    }
    protected override void OnEnable() {
        base.OnEnable();
        hitMonsters.Clear();
        if(currentLevel != originWeapon.CurrentLevel) {
            currentLevel = originWeapon.CurrentLevel;
            circleCollider.radius = DefaultColliderRadius * Scale;
            flyingAxeSprite.transform.localScale = new Vector3(Scale, Scale, Scale);
        }
        velocity = ((Vector2)transform.up + Vector2.up) * flyingSpeed;
    }
    private void OnDisable() {
        onDisable?.Invoke();
    }
    protected override void Update() {
        base.Update();
        velocity *= 1-Time.deltaTime;
        velocity = new Vector2(velocity.x, velocity.y + (-Time.deltaTime * 128f));
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == targetLayer.value
        && !hitMonsters.Contains(other.gameObject)) {
            Monster target;
            if(other.TryGetComponent<Monster>(out target)) {
                target.TakeDamage(Damage);
                target.TakeHittingDelay(hittingDelay);
                target.TakeForce(transform.up * 1f, hittingDelay);
                hitMonsters.Add(other.gameObject);
            }
        }
    }
}