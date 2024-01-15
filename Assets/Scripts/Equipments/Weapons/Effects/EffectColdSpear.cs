using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectColdSpear : EffectProjectile {
    private bool isActive = false;
    public WeaponColdSpear originWeapon;
    private float flyingSpeed = 31f;
    [SerializeField] private LayerMask targetLayer = 1<<8;
    [SerializeField] private CircleBounds damageAreaBounds;
    [SerializeField] private SpriteRenderer projectileRenderer;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    protected override void OnEnable() {
        base.OnEnable();
        projectileRenderer.enabled = true;
        isActive = true;
    }
    protected override void Update() {
        if(isActive) {
            transform.Translate(Vector2.up * flyingSpeed * Time.deltaTime);
            lifetime += Time.deltaTime * 5;
            if(lifetime >= flyingTime) {
                Disappear();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == targetLayer.value
        && isActive) {
            if(other.TryGetComponent(out Monster monster)) {
                monster.TakeDamage(originWeapon.Damage);
                var attch = originWeapon.AttachmentPooler.OutPool(this.transform.position, Quaternion.identity).GetComponent<AttachmentFreeze>();
                if(monster.TryGetAttachment(attch.AttachmentType, out Attachment already)) // Duplicate Attaching
                    monster.ReleaseAttachment(already);
                attch.duration = originWeapon.FreezeTime;
                monster.TakeAttachment(attch);
                originWeapon.SideEffectPooler.OutPool(monster.transform.position, Quaternion.identity);
                GameManager.instance.Character.OnAttackMonster(monster);
            }
        }
    }
    protected override void Disappear() {
        base.Disappear();
        projectileRenderer.enabled = false;
        isActive = false;
    }

    private void AttackArea() {
        Vector2 center = (Vector2)(transform.position + (Vector3)(transform.localToWorldMatrix * damageAreaBounds.center));
        Collider2D[] inners = Physics2D.OverlapCircleAll(center, damageAreaBounds.radius, targetLayer);
        for(int i=0; i<inners.Length; i++) {
            if(inners[i].TryGetComponent(out Monster monster)
            && monster.isArrive) {
                monster.TakeDamage(originWeapon.Damage);
                var attch = originWeapon.AttachmentPooler.OutPool(this.transform.position, Quaternion.identity).GetComponent<AttachmentFreeze>();
                if(monster.TryGetAttachment(attch.AttachmentType, out Attachment already)) // Duplicate Attaching
                    monster.ReleaseAttachment(already);
                attch.duration = originWeapon.FreezeTime;
                monster.TakeAttachment(attch);
                originWeapon.SideEffectPooler.OutPool(monster.transform.position, Quaternion.identity);
                GameManager.instance.Character.OnAttackMonster(monster);
            }
        }
    }
    private IEnumerator InPoolSideEffect(GameObject effect) {
        yield return new WaitForSeconds(3f);
        originWeapon.SideEffectPooler.InPool(effect);
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(damageAreaBounds.center, damageAreaBounds.radius);
    }
}