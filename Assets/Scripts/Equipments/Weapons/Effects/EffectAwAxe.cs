using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;

public class EffectAwAxe : EffectProjectile {
    public AWeaponMultiUseAxe originWeapon;

    private float flyingSpeed = 12f;
    private float maxVelocity = 0f;
    
    [SerializeField] LayerMask targetLayer = 1<<8;
    private float hitRadius = 1.25f;
    private float attackDelay = 0.15f;
    private float hitInterval = 0.13f;
    
    private float Damage => originWeapon.Damage;

    private Vector2 velocity;
    private bool isReturning;

    private Character character;
    private Character _Character {
        get {
            if(character == null)
                character = GameManager.instance.Character;
            return character;
        }
    }

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        
        velocity = transform.up * flyingSpeed;
        maxVelocity = flyingSpeed * 0.5f;
        isReturning = false;

        StartCoroutine(AttackCoroutine());
        StartCoroutine(ReturnCoroutine());
    }

    protected override void Update() {
        velocity += (Vector2)(_Character.transform.position - transform.position).normalized * 9f * Time.deltaTime;

        if(isReturning
        && velocity.magnitude > maxVelocity)
            velocity = velocity.normalized * maxVelocity;

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private IEnumerator AttackCoroutine() {
        GameManager.instance.Character.OnAttack();
        while(true) {
            var inners = Physics2D.OverlapCircleAll(transform.position, hitRadius, targetLayer.value);
            for(int i=0; i<inners.Length; i++) {
                if(inners[i].TryGetComponent(out Monster monster)) {
                    monster.TakeDamage(Damage * hitInterval);
                    monster.TakeAttackDelay(attackDelay);
                }
            }
            yield return new WaitForSeconds(hitInterval);
        }
    }
    private IEnumerator ReturnCoroutine() {
        yield return new WaitForSeconds(2f);
        isReturning = true;
    }

    protected override void Disapear() {
        base.Disapear();
        originWeapon.EffectPooler.InPool(this.gameObject);
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isReturning
        && (1<<other.gameObject.layer & 1<<GameManager.instance.Character.gameObject.layer) > 0) {
            Disapear();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}