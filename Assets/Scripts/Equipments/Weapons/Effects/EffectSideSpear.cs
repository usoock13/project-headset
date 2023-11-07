using System;
using UnityEngine;

public class EffectSideSpear : MonoBehaviour {
    public WeaponSpear originWeapon;
    private float hitDelay = 1f;
    private float attackForce = .4f;
    private float Damage => originWeapon.SideDamage;
    private float AreaScale => originWeapon.AreaScale;
    [SerializeField] private BoxBounds effectBounds;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private LayerMask targetLayer;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    private void OnEnable() {
        particle.Play();
        Vector3 center = transform.position + (Vector3)(transform.localToWorldMatrix * effectBounds.center);
        Collider2D[] inners = Physics2D.OverlapBoxAll(center, effectBounds.Size * AreaScale, transform.rotation.eulerAngles.z, targetLayer);
        foreach(Collider2D inner in inners) {
            var target = inner.GetComponent<IDamageable>();
            target.TakeDamage(Damage);
            target.TakeHittingDelay(hitDelay);
            target.TakeForce((inner.transform.position - originWeapon.transform.position).normalized * attackForce, hitDelay);
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(effectBounds.center, effectBounds.Size);
    }
}