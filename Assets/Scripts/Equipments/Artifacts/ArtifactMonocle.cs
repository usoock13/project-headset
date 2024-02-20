using UnityEngine;

public class ArtifactMonocle : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraRecovery = new float[] {  0.50f,  0.75f,  1.00f,  1.25f,  1.50f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Monocle",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Skill Gauge Recovery</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraRecovery[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Skill Gauge Recovery</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraRecovery[level-1]}</color> > <color=#f40>{extraRecovery[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "단안경",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>스킬 게이지 회복 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraRecovery[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>스킬 게이지 회복 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraRecovery[level-1]}</color> > <color=#f40>{extraRecovery[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraRecoveringSp += GetExtraRecovery;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraRecoveringSp -= GetExtraRecovery;
    }
    private float GetExtraRecovery(Character character) => extraRecovery[level-1];
}
