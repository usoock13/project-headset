using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactPlateBoots : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] bonusMoveSpeed = new float[]  { 0.30f,   0.45f,   0.60f,   0.75f,   0.90f };
    private float[] penaltyRecovery = new float[] { 0.30f,   0.25f,   0.20f,   0.15f,   0.10f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Plate Boots",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Move Speed</color> but gets decreasing <color=#f40>Stamina Recovery</color>."
                   + $"\n"
                   + $"\nIncreasing Speed : <color=#f40>{bonusMoveSpeed[0]}</color>"
                   + $"\nDecreasing Recovery : <color=#f40>{penaltyRecovery[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Move Speed</color> but gets decreasing <color=#f40>Stamina Recovery</color>."
                   + $"\n"
                   + $"\nIncreasing Speed : <color=#f40>{bonusMoveSpeed[level-1]}</color> > <color=#f40>{bonusMoveSpeed[NextLevelIndex]}</color>"
                   + $"\nDecreasing Recovery : <color=#f40>{penaltyRecovery[level-1]*100}%</color> > <color=#f40>{penaltyRecovery[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "판금 장화",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>이동 속도</color>을 얻지만 <color=#f40>스태미너 회복 속도</color>가 감소합니다."
                   + $"\n"
                   + $"\n이동 속도 증가량 : <color=#f40>{bonusMoveSpeed[0]}</color>"
                   + $"\n회복 속도 감소량 : <color=#f40>{penaltyRecovery[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>이동 속도</color>을 얻지만 <color=#f40>스태미너 회복 속도</color>가 감소합니다."
                   + $"\n"
                   + $"\n이동 속도 증가량 : <color=#f40>{bonusMoveSpeed[level-1]}</color> > <color=#f40>{bonusMoveSpeed[NextLevelIndex]}</color>"
                   + $"\n회복 속도 감소량 : <color=#f40>{penaltyRecovery[level-1]*100}%</color> > <color=#f40>{penaltyRecovery[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraMoveSpeed += GetExtraMoveSpeed;
        _Character.extraRecoveringStamina += GetExtraRecovery;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraMoveSpeed -= GetExtraMoveSpeed;
        _Character.extraRecoveringStamina -= GetExtraRecovery;
    }
    private float GetExtraMoveSpeed(Character character) => bonusMoveSpeed[level-1];
    private float GetExtraRecovery(Character character) => character.DefaultRecoveringStamina * -penaltyRecovery[level-1];
}