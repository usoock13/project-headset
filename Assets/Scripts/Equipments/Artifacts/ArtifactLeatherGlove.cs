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
        level switch {
            _ => $"<nobr>공격속도가 {extraSpeed[level]}만큼 증가합니다.</nobr>"
        };
    #endregion Artifact Information

    public override void OnEquipped() {
        _Character.extraAttackSpeed += GetExtraMoveSpeed;
    }
    private float GetExtraMoveSpeed(Character character) => character.DefaultMoveSpeed * extraSpeed[level-1];
}
