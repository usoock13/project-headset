using UnityEngine;

public class ArtifactBrace : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraPower = new float[] {  7f,   10f,   13f,   16f,   19f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "보호대";
    public override string Description => 
        NextLevelIndex switch {
            _ => $"<nobr>위력이 <color=#f40>{extraPower[NextLevelIndex]}</color>만큼 증가합니다.</nobr>"
        };
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraPower += GetExtraPower;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraPower -= GetExtraPower;
    }
    private float GetExtraPower(Character character) => character.DefaultMoveSpeed * extraPower[level-1];
}
