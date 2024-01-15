using System;
using System.Collections;
using UnityEngine;

public class AttachmentBurning : Attachment {
    [SerializeField] AWeaponElementalTrio originWeapon;
    
    private Monster owner;

    public override string AttachmentType => "Burning";
    public float damagePerSecond = 0f;
    public float duration = 0f;
    public float damageInterval = 0.5f;

    public Coroutine damageCoroutine;

    [SerializeField] private Color attachColor;

    public override void OnAttached(IAttachmentsTakeable target) {
        base.OnAttached(target);
        if(target.GameObject.TryGetComponent(out Monster monster)) {
            owner = monster;
            damageCoroutine = StartCoroutine(DamageCoroutine(owner));
            owner.ColorManager?.AddColor(attachColor);
        }
    }

    public override void OnDetached(IAttachmentsTakeable target) {
        base.OnDetached(target);
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        owner.ColorManager?.RemoveColor(attachColor);
        originWeapon.BurningPooler.InPool(this.gameObject);
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