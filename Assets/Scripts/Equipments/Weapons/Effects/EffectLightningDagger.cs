using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLightningDagger : EffectProjectile {
    private bool isActive = false;
    public WeaponLightningDagger originWeapon;

    private float Damage => originWeapon.Damage;
    private float flyingSpeed = 36f;
    private float hittingDelay = 0.4f;
    private float attackForceScalar = 0.5f;
    private float chainingHittingDelay = 0.5f;
    private float chainingRadius = 3f;

    [SerializeField] LayerMask targetLayer = 1<<8;
    [SerializeField] SpriteRenderer daggerRenderer;

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
        daggerRenderer.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isActive
        && 1<<other.gameObject.layer == targetLayer.value) {
            if(other.TryGetComponent(out Monster target)) {
                target.TakeDamage(Damage);
                target.TakeAttackDelay(hittingDelay);
                target.TakeForce(transform.up * attackForceScalar, hittingDelay);
                isActive = false;
                daggerRenderer.enabled = false;
                StartCoroutine(ChainLightning(target.gameObject));
                GameManager.instance.Character.OnAttackMonster(target);
            }
        }
    }

    private IEnumerator ChainLightning(GameObject hit) {
        GameObject last = hit.gameObject;
        Vector2 lastPoint = hit.transform.position;
        var lineRenderers = new List<LineRenderer>();
        
        for(int i=0;  i<originWeapon.ChainingCount;  i++){
            Collider2D[] inners = Physics2D.OverlapCircleAll(lastPoint, chainingRadius, targetLayer);
            foreach(var inner in inners) {
                if(last == inner.gameObject)
                    continue;
                if(inner.TryGetComponent(out Monster target)) {
                    target.TakeDamage(originWeapon.ChainingDamage);
                    target.TakeAttackDelay(chainingHittingDelay);
                    GameManager.instance.Character.OnAttackMonster(target);

                    LineRenderer lr = originWeapon.LineRendererPooler.OutPool().GetComponent<LineRenderer>();
                    lineRenderers.Add(lr);
                    if(lr is not null) {
                        lr.positionCount = 2;
                        lr.material.SetFloat("_Last_Start_Time", Time.time);
                        lr.SetPosition(0, lastPoint);
                        lr.SetPosition(1, target.transform.position);
                        last = target.gameObject;
                        lastPoint = target.transform.position;
                        yield return new WaitForSeconds(0.08f);
                        break;
                    }
                }
            }
        }
        yield return new WaitForSeconds(3f);

        foreach(var lr in lineRenderers)
            originWeapon.LineRendererPooler.InPool(lr.gameObject);
        Disapear();
    }
    protected override void Disapear() {
        base.Disapear();
        originWeapon.EffectPooler.InPool(this.gameObject);
    }
}