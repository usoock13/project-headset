using UnityEngine;

public class ArtifactPaperFan : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraRecovery = new float[] {  10f,   15f,   20f,   25f,   30f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Paper Fan",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Stamina Recovery</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraRecovery[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Stamina Recovery</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraRecovery[level-1]}</color> > <color=#f40>{extraRecovery[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "종이 부채",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>스태미너 회복 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraRecovery[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>스태미너 회복 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraRecovery[level-1]}</color> > <color=#f40>{extraRecovery[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraRecoveringStamina += GetExtraRecovery;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraRecoveringStamina -= GetExtraRecovery;
    }
    private float GetExtraRecovery(Character character) => extraRecovery[level-1];
}
