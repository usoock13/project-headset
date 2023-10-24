using UnityEngine;

public class EffectStraightPunch : MonoBehaviour {
    public WeaponOrthodox originWeapon;
    private float hitDelay = 0.75f;
    private float attackForceScalar = 1f;
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
        transform.Translate(effectBounds.center);
        particle.Play();
        Collider2D[] inners = Physics2D.OverlapBoxAll(transform.position, effectBounds.Size, transform.rotation.eulerAngles.z, targetLayer);
        foreach(Collider2D inner in inners) {
            var target = inner.GetComponent<IDamageable>();
            target.TakeDamage(originWeapon.DamageOfStraight);
            target.TakeForce((inner.transform.position - originWeapon.transform.position).normalized * attackForceScalar, hitDelay);
            target.TakeHittingDelay(hitDelay);
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(effectBounds.center, effectBounds.Size);
    }
}