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
    private readonly float maxIncreasing = 0.7f; // 70%

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Adrenaline",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets <color=#f40>Power</color> increases by lost HP."
                   + $"\n"
                   + $"\nPower per Lost HP : <color=#f40>{amountPerOnePercent[0]*100}%</color>"
                   + $"\nMaximum Increase : <color=#f40>{amountPerOnePercent[0]*100 * maxIncreasing*100}%</color>"
                   + $"<nobr>",
                _ => $"<nobr>"
                   + $"The character gets <color=#f40>Power</color> increases by lost HP."
                   + $"\n"
                   + $"\nPower per Lost HP : <color=#f40>{amountPerOnePercent[level-1]*100}%</color> > <color=#f40>{amountPerOnePercent[NextLevelIndex]*100}%</color>"
                   + $"\nMaximum Increase : <color=#f40>{amountPerOnePercent[level-1]*100 * maxIncreasing*100}%</color> > <color=#f40>{amountPerOnePercent[NextLevelIndex]*100 * maxIncreasing*100}%</color>"
                   + $"<nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "아드레날린",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 잃은 체력이 비례해 <color=#f40>위력</color>을 얻습니다."
                   + $"\n"
                   + $"\n잃은 체력당 위력 : <color=#f40>{amountPerOnePercent[0]*100}%</color>"
                   + $"\n최대 증가량 : <color=#f40>{amountPerOnePercent[0]*100 * maxIncreasing*100}%</color>"
                   + $"<nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 잃은 체력이 비례해 <color=#f40>위력</color>을 얻습니다."
                   + $"\n"
                   + $"\n잃은 체력당 위력 : <color=#f40>{amountPerOnePercent[level-1]*100}%</color> > <color=#f40>{amountPerOnePercent[NextLevelIndex]*100}%</color>"
                   + $"\n최대 증가량 : <color=#f40>{amountPerOnePercent[level-1]*100 * maxIncreasing*100}%</color> > <color=#f40>{amountPerOnePercent[NextLevelIndex]*100 * maxIncreasing*100}%</color>"
                   + $"<nobr>",
            }
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
    private float GetExtraPower(Character character) => Mathf.Min(1 - character.currentHp / character.MaxHp, maxIncreasing) * character.DefaultPower;
}
