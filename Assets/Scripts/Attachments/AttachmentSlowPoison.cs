using System;
using System.Collections;
using UnityEngine;

public class AttachmentSlowPoison : Attachment {
    public override string AttachmentType => "Slow Poison";

    public WeaponMaliciousFlask originWeapon;
    private float duration = 3f;
    public int attachmentLevel = 1;
    private float Damage => originWeapon.GetDamage(attachmentLevel);

    private float damageInterval = .5f;
    private float lifetime = 0;
    private Color attachedColor = new Color(.75f, 0, 1, 1);

    private Coroutine damageCoroutine;
    
    public override void OnAttached(IAttachmentsTakeable target) {
        lifetime = 0;
        attachmentLevel = originWeapon.CurrentLevel;

        #region Monster Target Implements
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            targetMonster.AddSpeedModifier(GetSpeedModifier);
            targetMonster.GetComponent<SpriteColorManager>()?.AddColor(attachedColor);
            damageCoroutine = StartCoroutine(DamageCoroutine(targetMonster));
        }
        #endregion Monster Target Implements
    }
    public override void OnDetached(IAttachmentsTakeable target) {
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            targetMonster.GetComponent<SpriteColorManager>()?.RemoveColor(attachedColor);
            targetMonster.RemoveSpeedModifier(GetSpeedModifier);
        }
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        originWeapon.AttachmentPooler.InPool(this.gameObject);
    }
    private IEnumerator DamageCoroutine(Monster target) {
        while(lifetime < duration) {
            target.TakeDamage(Damage * damageInterval);
            lifetime += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
        target.ReleaseAttachment(this);
    }
    private float GetSpeedModifier(Monster monster) {
        return 1-originWeapon.GetSlowAmount(attachmentLevel);
    }
}