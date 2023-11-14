using UnityEngine;

public class EffectOnePunch : MonoBehaviour {
    public WeaponOrthodox originWeapon;
    private float hitDelay = 0.25f;
    private float attackForceScalar = .1f;
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
        foreach(Collider2D inner in inners) {
            var target = inner.GetComponent<Monster>();
            target.TakeDamage(originWeapon.DamageOfOne);
            target.TakeHittingDelay(hitDelay);
            target.TakeForce((inner.transform.position - originWeapon.transform.position).normalized * attackForceScalar, hitDelay);
            GameManager.instance.Character.OnAttackMonster(target);
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(effectBounds.center, effectBounds.Size);
    }
}