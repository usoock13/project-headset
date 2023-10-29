using System;
using System.Collections;
using UnityEngine;

public class AttachmentSlowPoison : Attachment {
    public override string AttachmentType => "Slow Poison";

    private float damage = 2f;
    private Func<Monster, float> SlowAmount = (monster) => 0.5f;
    private float duration = 3f;
    private float damageInterval = .5f;
    private float lifetime = 0;

    private Coroutine damageCoroutine;
    
    public override void OnAttached(IAttachmentsTakeable target) {
        base.OnAttached(target);
        lifetime = 0;

        #region Monster Target Implements
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            targetMonster.moveSpeedScales += SlowAmount;
            damageCoroutine = StartCoroutine(DamageCoroutine(targetMonster));
        }
        #endregion Monster Target Implements
    }
    public override void OnDetached(IAttachmentsTakeable target) {
        base.OnDetached(target);
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            targetMonster.moveSpeedScales -= SlowAmount;
        }
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }
    private IEnumerator DamageCoroutine(Monster target) {
        while(lifetime < duration) {
            target.TakeDamage(damage);
            lifetime += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
        target.ReleaseAttachment(this);
    }
}