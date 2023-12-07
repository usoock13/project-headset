using UnityEngine;

public class ArtifactLeatherGlove : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraSpeed = new float[] {  0.20f,   0.25f,   0.30f,   0.35f,   0.40f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "가죽 장갑";
    public override string Description => 
        NextLevelIndex switch {
            _ => $"<nobr>공격속도가 <color=#f40>{extraSpeed[NextLevelIndex]}</color>만큼 증가합니다.</nobr>"
        };
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraAttackSpeed += GetExtraAttackSpeed;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraAttackSpeed -= GetExtraAttackSpeed;
    }
    private float GetExtraAttackSpeed(Character character) => character.DefaultMoveSpeed * extraSpeed[level-1];
}
