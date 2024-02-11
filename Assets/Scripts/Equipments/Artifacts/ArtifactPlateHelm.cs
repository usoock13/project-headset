using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactPlateHelm : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private int[] bonusHp = new int[]             {    20,      25,      30,      35,      40 };
    private float[] panaltyRecovery = new float[] { 0.30f,   0.25f,   0.20f,   0.15f,   0.10f };
    #endregion Artifact Status

    private int increased = 0;

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Plate Helm",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Max HP</color> but gets decreasing <color=#f40>Skill Gauge Recovery</color>."
                   + $"\n"
                   + $"\nIncreasing Max HP : <color=#f40>{bonusHp[0]}</color>"
                   + $"\nDecreasing Recovery : <color=#f40>{panaltyRecovery[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Max HP</color> but gets decreasing <color=#f40>Skill Gauge Recovery</color>."
                   + $"\n"
                   + $"\nIncreasing Max HP : <color=#f40>{bonusHp[level-1]}</color> > <color=#f40>{bonusHp[NextLevelIndex]}</color>"
                   + $"\nDecreasing Recovery : <color=#f40>{panaltyRecovery[level-1]*100}%</color> > <color=#f40>{panaltyRecovery[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "판금 투구",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>최대 체력</color>을 얻지만 <color=#f40>스킬 게이지 회복 속도</color>가 감소합니다."
                   + $"\n"
                   + $"\n최대 체력 증가량 : <color=#f40>{bonusHp[0]}</color>"
                   + $"\n회복 속도 감소량 : <color=#f40>{panaltyRecovery[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>최대 체력</color>을 얻지만 <color=#f40>스킬 게이지 회복 속도</color>가 감소합니다."
                   + $"\n"
                   + $"\n최대 체력 증가량 : <color=#f40>{bonusHp[level-1]}</color> > <color=#f40>{bonusHp[NextLevelIndex]}</color>"
                   + $"\n회복 속도 감소량 : <color=#f40>{panaltyRecovery[level-1]*100}%</color> > <color=#f40>{panaltyRecovery[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.IncreaseMaxHp(bonusHp[level-1] - increased);
        _Character.extraRecoveringStamina += GetExtraRecovery;
    }
    protected override void OnLevelUp() {
        base.OnLevelUp();
        _Character.IncreaseMaxHp(bonusHp[level-1] - increased);
        increased = bonusHp[level-1];
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.IncreaseMaxHp(-bonusHp[level-1]);
        _Character.extraRecoveringStamina -= GetExtraRecovery;
        increased = 0;
    }
    public float GetExtraRecovery(Character character) => -panaltyRecovery[level-1];
}
