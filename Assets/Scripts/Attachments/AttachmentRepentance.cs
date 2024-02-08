using System;
using System.Collections;
using UnityEngine;

public class AttachmentRepentance : Attachment {
    private Monster owner;
    [SerializeField] private AweaponHolyLand originWeapon;
    public override string AttachmentType => "Repentance";

    private float duration = 0.6f;

    private Coroutine livingCoroutine;
    
    public override void OnAttached(IAttachmentsTakeable target) {
        
        if(target.GameObject.TryGetComponent(out Monster monster)) {
            owner = monster;
            livingCoroutine = StartCoroutine(DurationCoroutine());
        }
    }
    public override void OnDetached(IAttachmentsTakeable target) {
        if(livingCoroutine != null)
            StopCoroutine(livingCoroutine);
        owner.onDie -= OnMonsterDie;
    }

    private IEnumerator DurationCoroutine() {
        owner.onDie += OnMonsterDie;
        yield return new WaitForSeconds(duration);
        owner.ReleaseAttachment(this);
    }

    private void OnMonsterDie(Monster monster) {
        originWeapon.OnKillMonster();
    }
}