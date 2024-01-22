using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactPlateArmor : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] armorAmount = new float[]     {    24,      28,      32,      36,      40 };
    private float[] decreasingSpeed = new float[] { 0.30f,   0.25f,   0.20f,   0.15f,   0.10f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Plate Armor",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Armor</color> but gets decreasing <color=#f40>Movement Speed</color>."
                   + $"\n"
                   + $"\nIncreasing Armor : <color=#f40>{armorAmount[0]}</color>"
                   + $"\nDecreasing Speed : <color=#f40>{decreasingSpeed[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Armor</color> but gets decreasing <color=#f40>Movement Speed</color>."
                   + $"\n"
                   + $"\nIncreasing Armor : <color=#f40>{armorAmount[level-1]}</color> > <color=#f40>{armorAmount[NextLevelIndex]}</color>"
                   + $"\nDecreasing Speed : <color=#f40>{decreasingSpeed[level-1]*100}%</color> > <color=#f40>{decreasingSpeed[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "판금 흉갑",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>방어력</color>을 얻지만 <color=#f40>이동 속도</color>가 감소합니다."
                   + $"\n"
                   + $"\n방어력 증가량 : <color=#f40>{armorAmount[0]}</color>"
                   + $"\n속도 감소량 : <color=#f40>{decreasingSpeed[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>방어력</color>을 얻지만 <color=#f40>이동 속도</color>가 감소합니다."
                   + $"\n"
                   + $"\n방어력 증가량 : <color=#f40>{armorAmount[level-1]}</color> > <color=#f40>{armorAmount[NextLevelIndex]}</color>"
                   + $"\n속도 감소량 : <color=#f40>{decreasingSpeed[level-1]*100}%</color> > <color=#f40>{decreasingSpeed[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraArmor += GetExtraArmor;
        _Character.extraMoveSpeed += GetExtraMoveSpeed;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraArmor -= GetExtraArmor;
        _Character.extraMoveSpeed -= GetExtraMoveSpeed;
    }
    private float GetExtraArmor(Character character) => armorAmount[level-1];
    private float GetExtraMoveSpeed(Character character) => character.DefaultMoveSpeed * -decreasingSpeed[level-1];
}
