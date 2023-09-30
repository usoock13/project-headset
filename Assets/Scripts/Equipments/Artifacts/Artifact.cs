using UnityEngine;

public abstract class Artifact : Equipment {
    protected Character Character { get { return GameManager.instance.Character; } }

    public override void OnGotten() {
        Character.AddArtifact(this);
        OnEquipped();
    }
    public override void OnEquipped() {
        this.gameObject.SetActive(true);
    }
}