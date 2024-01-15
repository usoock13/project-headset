using UnityEngine;

public class ArtifactLeatherGlove : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraSpeed = new float[] {  0.2f,   0.25f,   0.30f,   0.35f,   0.40f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Leather Glove",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Attack Speed</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraSpeed[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Attack Speed</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraSpeed[level-1]}</color> > <color=#f40>{extraSpeed[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
        
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "가죽 장갑",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>공격 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraSpeed[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>공격 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraSpeed[level-1]}</color> > <color=#f40>{extraSpeed[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraAttackSpeed += GetExtraAttackSpeed;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraAttackSpeed -= GetExtraAttackSpeed;
    }
    private float GetExtraAttackSpeed(Character character) => extraSpeed[level-1];
}
