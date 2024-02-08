using System;
using UnityEngine;

public class AttachmentStun : Attachment {
    public override string AttachmentType => throw new NotImplementedException();
    private float duration = 1f;
    private Character owner = null;

    public override void OnAttached(IAttachmentsTakeable target) {
        var character = target.GameObject.GetComponent<Character>();
        if(character != null) {
            owner = character;
            character.canMove = false;
        }
    }

    public override void OnDetached(IAttachmentsTakeable target) {
        owner.canMove = true;
    }
}