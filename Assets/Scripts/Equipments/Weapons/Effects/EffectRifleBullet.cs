using System.Collections.Generic;
using UnityEngine;

public class EffectRifleBullet : EffectProjectile {
    public WeaponRifle originWeapon;
    private float Damage => originWeapon.Damage;
    private float flyingSpeed = 36f;
    private float hittingDelay = 0.4f;
    private const float FLYING_TIME = 3;
    [SerializeField] LayerMask targetLayer = 8;
    private List<GameObject> hitMonsters = new List<GameObject>();

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
    private void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == targetLayer.value
        && !hitMonsters.Contains(other.gameObject)) {
            if(other.TryGetComponent<Monster>(out Monster target)) {
                target.TakeDamage(Damage);
                target.TakeHittingDelay(hittingDelay);
                target.TakeForce(transform.up * 1f, hittingDelay);
                hitMonsters.Add(other.gameObject);
                GameManager.instance.Character.OnAttackMonster(target);
            }
        }
    }
}