using System;
using UnityEngine;

public interface IAttachmentsTakeable {
    public GameObject GameObject { get; }
    public void TakeAttachment(Attachment attachment);
    public void ReleaseAttachment(Attachment attachment);
    public bool TryGetAttachment(string attahcmentType, out Attachment attachment);
}