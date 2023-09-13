using UnityEngine;

public abstract class Artifact : Equipment {
    public override void OnEquipped() {
        GameManager.instance.Character.AddArtifact(this);
    }
}