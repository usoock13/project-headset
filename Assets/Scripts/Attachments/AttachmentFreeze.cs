using System;
using System.Collections;
using UnityEngine;

public class AttachmentFreeze : Attachment {
    public override string AttachmentType => "Freeze";

    public float duration  = 1f;
    private float lifetime = 0;
    private Color attachColor = new Color(.2f, .6f, 1f);

    private Coroutine detachCoroutine;
    
    public override void OnAttached(IAttachmentsTakeable target) {
        base.OnAttached(target);
        lifetime = 0;

        #region Monster Target Implements
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            targetMonster.AddSpeedModifier(GetSpeedModifier);
            targetMonster.GetComponent<SpriteColorManager>()?.AddColor(attachColor);
            detachCoroutine = StartCoroutine(DetachCoroutine(targetMonster));
        }
        #endregion Monster Target Implements
    }
    public override void OnDetached(IAttachmentsTakeable target) {
        base.OnDetached(target);
        if(target.GameObject.TryGetComponent(out Monster targetMonster)) {
            targetMonster.ColorManager?.RemoveColor(attachColor);
            targetMonster.RemoveSpeedModifier(GetSpeedModifier);
        }
        if(detachCoroutine != null)
            StopCoroutine(detachCoroutine);
    }
    private IEnumerator DetachCoroutine(Monster target) {
        while(lifetime < duration) {
            lifetime += Time.deltaTime;
            yield return null;
        }
        target.ReleaseAttachment(this);
    }
    private float GetSpeedModifier(Monster monster) {
        return 0;
    }
}