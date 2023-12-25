using System;
using System.Collections;
using UnityEngine;

public class AttachmentBurning : Attachment {
    public override string AttachmentType => "Burning";
    public float damagePerSecond = 0f;
    public float duration = 0f;
    public float damageInterval = 0.5f;

    public Coroutine damageCoroutine;

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
            monster.TakeDamage(damagePerSecond * damageInterval);
            lifetime += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
    }
}