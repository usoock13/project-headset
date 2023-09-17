using JetBrains.Annotations;
using UnityEngine;

public class EffectLongbow : MonoBehaviour {
    public bool isActive = false;
    public WeaponLongbow originWeapon;
    private float Damage => originWeapon.Damage;
    private float flyingSpeed = 10f;
    private float hittingkDelay = 0.4f;
    [SerializeField] LayerMask targetLayer = 8;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    private void Update() {
        transform.Translate(Vector2.up * flyingSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == targetLayer.value) {
            Monster target;
            if(other.TryGetComponent<Monster>(out target)) {
                target.TakeDamage(Damage);
                target.TakeHittingDelay(hittingkDelay);
                target.TakeForce(transform.up * 5f);
            }
        }
    }
}