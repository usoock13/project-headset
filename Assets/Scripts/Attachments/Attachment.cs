using System;
using UnityEngine;

public abstract class Attachment : MonoBehaviour {
    public abstract string AttachmentType { get; }

    public Action<Attachment> onAttached;
    public Action<Attachment> onDetached;

    private Sprite _icon;
    private string _name;
    private string _description;
    private string _abstract;
    
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
    public string Abstract => _abstract;

    public virtual void OnAttached(IAttachmentsTakeable target) {
        onAttached?.Invoke(this);
    }
    public virtual void OnDetached(IAttachmentsTakeable target) {
        onDetached?.Invoke(this);
    }
    public static bool operator == (Attachment a, Attachment b) {
        if(a is null || b is null)
            return a is null&&b is null ? true : false;
        return a.AttachmentType == b.AttachmentType;
    }
    public static bool operator != (Attachment a, Attachment b) {
        if(a==null || b==null)
            return a==null&&b==null ? true : false;
        return a.AttachmentType != b.AttachmentType;
    }
    public override bool Equals(object o) {
        Attachment a = o as Attachment;
        if(a is null)
            return false;
        return this.AttachmentType == a.AttachmentType;
    }
    public override int GetHashCode() {
        return this.AttachmentType.GetHashCode();
    }
}