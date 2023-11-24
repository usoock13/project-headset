using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactAdrenaline : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] amountPerOnePercent = new float[] { 0.0_040f, 0.0_055f, 0.0_070f, 0.0_085f, 0.0_100f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "아드레날린 주사";
    public override string Description => 
        level switch {
            _ => $"잃은 체력 1%마다 {amountPerOnePercent[level] * 100}%의 추가 위력을 얻으며, 최대 {amountPerOnePercent[level] * 70 * 100}%까지 증가합니다."
        };
    #endregion Artifact Information

    public override void OnEquipped() {
        _Character.extraPower += GetExtraPower;
    }
    private float GetExtraPower(Character character) => Mathf.Min((1 - character.currentHp / character.MaxHp), 0.7f) * character.DefaultPower;
}
