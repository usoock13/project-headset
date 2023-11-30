using System;
using System.Collections;
using UnityEngine;

public class AttachmentFreeze : Attachment {
    public override string AttachmentType => "Freeze";

    public WeaponColdSpear originWeapon;
    private float Duration => originWeapon.Duration;
    public int attachmentLevel = 1;
    private float lifetime = 0;
    private Color attachedColor = new Color(.2f, .6f, 1f);

    private Coroutine detachCoroutine;
    
    public override void OnAttached(IAttachmentsTakeable target) {
        base.OnAttached(target);
        lifetime = 0;
        attachmentLevel = originWeapon.CurrentLevel;

        #region Monster Target Implements
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            targetMonster.AddSpeedModifier(GetSpeedModifier);
            targetMonster.GetComponent<SpriteColorManager>()?.AddColor(attachedColor);
            detachCoroutine = StartCoroutine(DetachCoroutine(targetMonster));
        }
        #endregion Monster Target Implements
    }
    public override void OnDetached(IAttachmentsTakeable target) {
        base.OnDetached(target);
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            targetMonster.GetComponent<SpriteColorManager>()?.RemoveColor(attachedColor);
            targetMonster.RemoveSpeedModifier(GetSpeedModifier);
        }
        if(detachCoroutine != null)
            StopCoroutine(detachCoroutine);
    }
    private IEnumerator DetachCoroutine(Monster target) {
        while(lifetime < Duration) {
            lifetime += Time.deltaTime;
            yield return null;
        }
        target.ReleaseAttachment(this);
    }
    private float GetSpeedModifier(Monster monster) {
        return 0;
    }
}