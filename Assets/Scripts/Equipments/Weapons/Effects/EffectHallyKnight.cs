using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHallyKnight : MonoBehaviour
{
    [SerializeField] AweaponHolyLand originWeapon;
    private Vector2 moveDir;
    [SerializeField] private LayerMask targetLayer = 1<<8;
    [SerializeField] new private Collider2D collider;

    private float damage;
    private float duration;
    private float attackDelay = 0.25f;
    private float moveSpeed = 2f;

    private float attackInterval = 0.5f;
    
    private void Update() {
        if(moveDir != Vector2.zero)
            transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    public void Active(float damage, float duration) {
        this.damage = damage;
        this.duration = duration;
        StartCoroutine(DisappearCoroutine());
        StartCoroutine(UpdateDirection());
        StartCoroutine(AttackAround());
    }

    private IEnumerator DisappearCoroutine() {
        yield return new WaitForSeconds(duration);
        Disappear();
    }

    private IEnumerator UpdateDirection() {
        while(true) {
            Vector2 cPoint = GameManager.instance.Character.transform.position;
            Vector2 nextPoint = cPoint + new Vector2(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
            moveDir = (nextPoint - (Vector2)transform.position).normalized;
            yield return new WaitForSeconds(3f);
        }
    }
    
    private IEnumerator AttackAround() {
        while(true) {
            var inners = new List<Collider2D>();
            var filter = new ContactFilter2D() {
                layerMask = targetLayer.value,
                useLayerMask = true,
            };
            Physics2D.OverlapCollider(collider, filter, inners);
            for(int i=0; i<inners.Count; i++) {
                if(inners[i].TryGetComponent(out Monster monster)) {
                    monster.TakeDamage(damage * attackInterval);
                    monster.TakeStagger(attackDelay);

                    GameManager.instance.Character.OnAttackMonster(monster);
                }
            }
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void Disappear() {
        moveDir = Vector2.zero;
        StopAllCoroutines();
        originWeapon.KnightPooler.InPool(this.gameObject);
    }
}
