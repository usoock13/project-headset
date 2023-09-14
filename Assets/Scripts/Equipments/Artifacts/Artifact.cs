using UnityEngine;

public abstract class Artifact : Equipment {
    public override void OnEquipped() {
        this.gameObject.SetActive(true);
    }
}