using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EffectWhip : MonoBehaviour {
    public WeaponWhip originWeapon;
    [SerializeField] private float distance = 2;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private PolygonCollider2D attackCollider;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        StartCoroutine(AttackProcess());
    }

    private System.Collections.IEnumerator AttackProcess() {
        yield return null;
        particle.Play();
        var inners = new List<Collider2D>();
        Physics2D.OverlapCollider(
            collider: attackCollider,
            contactFilter:
                new ContactFilter2D() {
                    layerMask = 1<<targetLayer
                }, 
            results: inners
        );
        float angle = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;
        float swipeDir = transform.localScale.x > 0  ?  1  :  -1;
        foreach(var inner in inners) {
            if(inner.TryGetComponent(out Monster monster)) {
                Vector2 dir = monster.transform.position - transform.position;
                dir = Quaternion.AngleAxis(-angle, Vector3.forward) * dir;
                float distance = Vector2.Distance(monster.transform.position, transform.position);
                Vector2 attackForce = transform.right * Mathf.Sin( Mathf.Atan2(dir.y, dir.x) ) * distance;

                monster.TakeDamage(originWeapon.Damage);
                monster.TakeHittingDelay(originWeapon.HittingDelay);
                if(swipeDir * Mathf.Atan2(dir.y, dir.x) < 0)
                    monster.TakeForce(attackForce, 0.5f);
            }
        }
        yield return new WaitForSeconds(3f);
        originWeapon.EffectPooler.InPool(this.gameObject);
    }
}