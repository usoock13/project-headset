using System;
using System.Collections;
using UnityEngine;

public class AttachmentBleed : Attachment {
    public override string AttachmentType => "Bleed";

    public WeaponMaliciousFlask originWeapon;
    private float duration = 5f;
    private int duplicatedCount = 0;
    public int attachmentLevel = 1;
    private float Damage => originWeapon.GetDamage(attachmentLevel);

    private float damageInterval = .5f;
    private float lifetime = 0;

    private Coroutine damageCoroutine;
    
    public override void OnAttached(IAttachmentsTakeable target) {
        lifetime = 0;
        attachmentLevel = originWeapon.CurrentLevel;

        #region Monster Target Implements
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            damageCoroutine = StartCoroutine(DamageCoroutine(targetMonster));
        }
        #endregion Monster Target Implements
    }
    public override void OnDetached(IAttachmentsTakeable target) {
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }
    private IEnumerator DamageCoroutine(Monster target) {
        while(lifetime < duration) {
            target.TakeDamage(Damage);
            lifetime += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
        target.ReleaseAttachment(this);
    }
}