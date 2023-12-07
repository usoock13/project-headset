using UnityEngine;

public abstract class Artifact : Equipment {
    protected Character _Character { get { return GameManager.instance.Character; } }

    protected override void OnLevelUp() {
        base.OnLevelUp();
        GameManager.instance.StageManager._StageUIManager.UpdateArtifactList();
    }
    public override void OnGotten() {
        OnEquipped();
    }
    public override void OnEquipped() {
        this.gameObject.SetActive(true);
    }
    public override void OnTakeOff() {
        this.gameObject.SetActive(false);
    }
}