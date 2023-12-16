using System;
using System.Collections;
using UnityEngine;

public class EffectAwExplosionArrow : EffectProjectile {
    private bool isActive = false;
    public AWeaponExplosionArrow originWeapon;
    private float Damage => originWeapon.BombDamage;
    private float flyingSpeed = 36f;
    private float hittingDelay = 1.5f;
    private float explostionRadius = 1.25f;
    [SerializeField] LayerMask targetLayer = 1<<8;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] ParticleSystem explosionParticle;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    
    protected override void Update() {
        if(isActive) {
            base.Update();
            transform.Translate(Vector2.up * flyingSpeed * Time.deltaTime);
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        isActive = true;
        trailRenderer.Clear();
        spriteRenderer.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive
        && 1<<other.gameObject.layer == targetLayer.value) {
            if(other.TryGetComponent(out Monster monster)) {
                isActive = false;
                Explode();
            }
        }
    }

    private void Explode() {
        Collider2D[] inners = Physics2D.OverlapCircleAll(transform.position, explostionRadius, targetLayer);
        for(int i=0; i<inners.Length; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(Damage);
                monster.TakeAttackDelay(hittingDelay);
                monster.TakeForce((monster.transform.position - transform.position).normalized * 4f, hittingDelay);
                GameManager.instance.Character.OnAttackMonster(monster);
                explosionParticle.Play();
                spriteRenderer.enabled = false;
                StartCoroutine(DisapearCoroutine());
                isActive = false;
            }
        }
    }

    private IEnumerator DisapearCoroutine() {
        yield return new WaitForSeconds(3f);
        Disapear();
    }

    protected override void Disapear() {
        base.Disapear();
        originWeapon.ExplosionEffectPooler.InPool(this.gameObject);
    }
}