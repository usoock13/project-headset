using System;
using UnityEngine;

public class EffectSpear : MonoBehaviour {
    public WeaponSpear originWeapon;
    private float hitDelay = 1f;
    private float attackForce = .8f;
    private float Damage => originWeapon.Damage;
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
        Collider2D[] inners = Physics2D.OverlapBoxAll(center, effectBounds.Size, transform.rotation.eulerAngles.z, targetLayer);
        foreach(Collider2D inner in inners) {
            if(inner.TryGetComponent(out Monster target)) {
                target.TakeDamage(Damage);
                target.TakeStagger(hitDelay);
                target.TakeForce((inner.transform.position - originWeapon.transform.position).normalized * attackForce, hitDelay);
                GameManager.instance.Character.OnAttackMonster(target);
            }
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(effectBounds.center, effectBounds.Size);
    }
}