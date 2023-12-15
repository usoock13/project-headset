using UnityEngine;

public class ArtifactLeatherGlove : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraSpeed = new float[] {  0.15f,   0.18f,   0.21f,   0.24f,   0.27f };
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
