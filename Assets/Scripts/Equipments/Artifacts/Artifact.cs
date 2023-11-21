using UnityEngine;

public abstract class Artifact : Equipment {
    protected Character _Character { get { return GameManager.instance.Character; } }

    public override void OnGotten() {
        OnEquipped();
    }
    public override void OnEquipped() {
        this.gameObject.SetActive(true);
    }
}