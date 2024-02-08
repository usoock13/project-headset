using System;
using System.Collections;
using UnityEngine;

public class AttachmentCorrosion : Attachment {
    public override string AttachmentType => "Corrosion";

    public AWeaponAcidFlask originWeapon;
    float lifetime = 0;

    private int duplicatingCount = 0;
    private readonly int maxDuplicatedCount = 5;

    private float damageInterval = 0.5f;
    private Color attachedColor = new Color(.88f, 1, 0, 1);

    private Coroutine damageCoroutine;
    
    public override void OnAttached(IAttachmentsTakeable target) {
        lifetime = 0;
        duplicatingCount = 1;
        
        if(target.GameObject.TryGetComponent(out Monster monster)) {
            monster.AddSpeedModifier(GetSpeedModifier);
            monster.GetComponent<SpriteColorManager>()?.AddColor(attachedColor);
            monster.extraDamageScale += ExtraDamageScale;
            damageCoroutine = StartCoroutine(DamageCoroutine(monster));
        }
    }
    public override void OnDetached(IAttachmentsTakeable target) {
        duplicatingCount = 0;
        
        if(target.GameObject.TryGetComponent(out Monster monster)) {
            monster.RemoveSpeedModifier(GetSpeedModifier);
            monster.GetComponent<SpriteColorManager>()?.RemoveColor(attachedColor);
            monster.extraDamageScale -= ExtraDamageScale;
        }
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }

    public void Duplicate() {
        if(duplicatingCount < maxDuplicatedCount)
            duplicatingCount ++;
    }

    private IEnumerator DamageCoroutine(Monster target) {
        float duration = 5f;
        while(lifetime < duration) {
            target.TakeDamage(originWeapon.Damage * duplicatingCount * damageInterval);
            lifetime += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
        target.ReleaseAttachment(this);
    }

    private float ExtraDamageScale(Monster monster) => originWeapon.extraDamageScale * duplicatingCount;
    private float GetSpeedModifier(Monster monster) => 1-originWeapon.slowAmount;
}