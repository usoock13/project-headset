using System.Collections.Generic;
using UnityEngine;

public class EffectMaliciousFlask : EffectProjectile {
    private bool isActive = false;
    public WeaponMaliciousFlask originWeapon;
    private float Damage => originWeapon.Damage;
    private float flyingSpeed = 45f;
    private float currentSpeed = 0;
    [SerializeField] private LayerMask targetLayer = 8;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private CircleBounds damageAreaBounds;
    [SerializeField] private SpriteRenderer flaskSpriteRenderer;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    protected override void OnEnable() {
        base.OnEnable();
        currentSpeed = flyingSpeed;
        flaskSpriteRenderer.enabled = true;
        isActive = true;
    }
    protected override void Update() {
        if(isActive) {
            transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
            currentSpeed = Mathf.Lerp(flyingSpeed, 0, lifetime/flyingTime);
            lifetime += Time.deltaTime * 5;
            if(lifetime >= flyingTime) {
                Disapear();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(1<<other.gameObject.layer == targetLayer.value
        && isActive) {
            Monster target;
            if(other.TryGetComponent<Monster>(out target)) {
                Disapear();
            }
        }
    }
    protected override void Disapear() {
        base.Disapear();
        flaskSpriteRenderer.enabled = false;
        explosionEffect.Play();
        AttackArea();
        isActive = false;
    }
    private void AttackArea() {
        Vector2 center = (Vector2)(transform.position + (Vector3)(transform.localToWorldMatrix * damageAreaBounds.center));
        Collider2D[] inners = Physics2D.OverlapCircleAll(center, damageAreaBounds.radius, targetLayer);
        for(int i=0; i<inners.Length; i++) {
            if(inners[i].TryGetComponent(out Monster target)
            && target.isArrive) {
                AttachmentSlowPoison attachment = originWeapon.AfterAttahcment;
                if(target.TryGetAttachment(attachment.AttachmentType, out Attachment already))
                    target.ReleaseAttachment(already);
                target.TakeAttachment(attachment);
                GameManager.instance.Character.OnAttackMonster(target);
            }
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(damageAreaBounds.center, damageAreaBounds.radius);
    }
}