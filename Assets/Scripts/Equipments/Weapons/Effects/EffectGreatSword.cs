using UnityEngine;

public class EffectGreatSword : MonoBehaviour {
    public WeaponGreatSword originWeapon;
    private float hitDelay = 0.6f;
    private float attackForceScalar = 2f;
    [SerializeField] private BoxBounds effectBounds = new BoxBounds(new Vector2(0, 0), new Vector2(3f, 1.5f));
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
            target.TakeDamage(originWeapon.Damage);
            target.TakeHittingDelay(hitDelay);
            target.TakeForce(transform.up * attackForceScalar, hitDelay);
            GameManager.instance.Character.OnAttackMonster(target);
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(effectBounds.center, effectBounds.Size);
    }
}