using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAwMagicPen : MonoBehaviour {
    [SerializeField] private AWeaponMagicPen originWeapon;
    [SerializeField] new private BoxCollider2D collider;
    [SerializeField] private LayerMask targetLayer = 1<<8;
    
    private Coroutine attackCoroutine;

    public void AttackForward() {
        if(attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public IEnumerator AttackCoroutine() {
        transform.Translate(new Vector2(0, UnityEngine.Random.Range(0f, 1f)));
        var inners = new List<Collider2D>();
        var filter = new ContactFilter2D() {
            layerMask = targetLayer.value
        };
        Physics2D.OverlapCollider(collider, filter, inners);
        for(int i=0; i<inners.Count; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(originWeapon.Damage * originWeapon.Interval);
                monster.TakeAttackDelay(originWeapon.Interval * 0.5f);
            }
        }
        yield return new WaitForSeconds(originWeapon.Interval * 0.5f);
        transform.position = originWeapon.transform.position + Vector3.up * 0.5f;
    }
}
