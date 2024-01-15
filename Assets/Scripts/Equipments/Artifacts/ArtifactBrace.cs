using UnityEngine;

public class ArtifactBrace : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraPower = new float[] {  8f,   11f,   14f,   17f,   20f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Brace",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Power</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraPower[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Power</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraPower[level-1]}</color> > <color=#f40>{extraPower[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "보호대",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>위력</color>을 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraPower[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>위력</color>을 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraPower[level-1]}</color> > <color=#f40>{extraPower[NextLevelIndex]}</color>"
                   + $"</nobr>",
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
    private float GetExtraPower(Character character) => extraPower[level-1];
}
