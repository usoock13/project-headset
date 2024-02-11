using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactPlateGloves : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] bonusPower = new float[]  {    15,      20,      25,      30,      35 };
    private float[] penaltySlow = new float[] { 0.30f,   0.25f,   0.20f,   0.15f,   0.10f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Plate Armor",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Power</color> but gets decreasing <color=#f40>Attack Speed</color>."
                   + $"\n"
                   + $"\nIncreasing Power : <color=#f40>{bonusPower[0]}</color>"
                   + $"\nDecreasing Speed : <color=#f40>{penaltySlow[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Power</color> but gets decreasing <color=#f40>Attack Speed</color>."
                   + $"\n"
                   + $"\nIncreasing Power : <color=#f40>{bonusPower[level-1]}</color> > <color=#f40>{bonusPower[NextLevelIndex]}</color>"
                   + $"\nDecreasing Speed : <color=#f40>{penaltySlow[level-1]*100}%</color> > <color=#f40>{penaltySlow[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "판금 장갑",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>위력</color>을 얻지만 <color=#f40>공격 속도</color>가 감소합니다."
                   + $"\n"
                   + $"\n위력 증가량 : <color=#f40>{bonusPower[0]}</color>"
                   + $"\n속도 감소량 : <color=#f40>{penaltySlow[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>위력</color>을 얻지만 <color=#f40>공격 속도</color>가 감소합니다."
                   + $"\n"
                   + $"\n위력 증가량 : <color=#f40>{bonusPower[level-1]}</color> > <color=#f40>{bonusPower[NextLevelIndex]}</color>"
                   + $"\n속도 감소량 : <color=#f40>{penaltySlow[level-1]*100}%</color> > <color=#f40>{penaltySlow[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraPower += GetExtraPower;
        _Character.extraAttackSpeed += GetExtraAttackSpeed;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraPower -= GetExtraPower;
        _Character.extraAttackSpeed -= GetExtraAttackSpeed;
    }
    private float GetExtraPower(Character character) => bonusPower[level-1];
    private float GetExtraAttackSpeed(Character character) => -penaltySlow[level-1];
}