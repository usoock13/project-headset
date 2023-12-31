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

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Adrenaline",
        Description: 
            $"<nobr>잃은 체력 1%마다 <color=#f40>{amountPerOnePercent[NextLevelIndex] * 100}%</color>의 추가 위력을 얻으며, 최대 <color=#f40>{amountPerOnePercent[NextLevelIndex] * 70 * 100}%</color>까지 증가합니다.</nobr>"
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "아드레날린",
        Description: 
            $"<nobr>잃은 체력 1%마다 <color=#f40>{amountPerOnePercent[NextLevelIndex] * 100}%</color>의 추가 위력을 얻으며, 최대 <color=#f40>{amountPerOnePercent[NextLevelIndex] * 70 * 100}%</color>까지 증가합니다.</nobr>"
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraPower += GetExtraPower;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraPower -= GetExtraPower;
    }
    private float GetExtraPower(Character character) => Mathf.Min((1 - character.currentHp / character.MaxHp), 0.7f) * character.DefaultPower;
}
