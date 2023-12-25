using System;
using System.Collections;
using UnityEngine;

public class AttachmentShock : Attachment {
    public override string AttachmentType => "Shock";
    public float damagePerSecond = 0f;
    public float duration = 0f;
    public float damageInterval = 1f;
    public float chainRadius = 2f;
    public int maxChainCount = 3;

    [SerializeField] private LayerMask chainLayerMask = 1<<8;

    [SerializeField] private LineRenderer chainRenderer;

    private Coroutine damageCoroutine;

    public override void OnAttached(IAttachmentsTakeable target) {
        base.OnAttached(target);
        if(target.GameObject.TryGetComponent(out Monster monster)) {
            damageCoroutine = StartCoroutine(DamageCoroutine(monster));
        }
    }

    public override void OnDetached(IAttachmentsTakeable target) {
        base.OnDetached(target);
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }

    private IEnumerator DamageCoroutine(Monster monster) {
        float lifetime = 0f;
        while(lifetime < duration) {
            StartCoroutine(DamageAround());
            lifetime += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
    }
    
    private IEnumerator DamageAround() {
        var inners = Physics2D.OverlapCircleAll(transform.position, chainRadius, chainLayerMask);
        int count = 0;
        for(int i=0; i<inners.Length; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(damagePerSecond * damageInterval);
                monster.TakeAttackDelay(0.6f);
                chainRenderer.positionCount = 2;
                chainRenderer.SetPositions(new Vector3[] {transform.position, monster.transform.position});
                chainRenderer.material.SetFloat("_Last_Start_Time", Time.time);
                count ++;
                if(count >= maxChainCount)
                    break;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}