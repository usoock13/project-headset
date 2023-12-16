using UnityEngine;

public class EffectAwMetalPunch : MonoBehaviour {
    public AWeaponAutomail originWeapon;
    private float hitDelay = 0.15f;
    private float attackForceScalar = 0.33f;
    [SerializeField] private BoxBounds effectBounds;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] LayerMask targetLayer;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    private void OnEnable() {
        particle.Play();
        Vector3 center = transform.position + (Vector3)(transform.localToWorldMatrix * effectBounds.center);
        Collider2D[] inners = Physics2D.OverlapBoxAll(center, effectBounds.Size, transform.rotation.eulerAngles.z, targetLayer);
        if(inners.Length > 0)
            originWeapon.OnAttackMonster();
        foreach(Collider2D inner in inners) {
            if(inner.TryGetComponent(out Monster target)) {
                target.TakeDamage(originWeapon.Damage);
                target.TakeAttackDelay(hitDelay);
                target.TakeForce(transform.up * attackForceScalar, hitDelay);
                GameManager.instance.Character.OnAttackMonster(target);
            }
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(effectBounds.center, effectBounds.Size);
    }
}