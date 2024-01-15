using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactClothArmor : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraArmor = new float[] {  11f,   15f,   19f,   23f,   27f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Cloth Armor",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Armor</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraArmor[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Armor</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraArmor[level-1]}</color> > <color=#f40>{extraArmor[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "천 갑옷",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>방어력</color>을 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraArmor[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>방어력</color>을 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraArmor[level-1]}</color> > <color=#f40>{extraArmor[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraArmor += GetExtraMoveSpeed;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraArmor -= GetExtraMoveSpeed;
    }
    private float GetExtraMoveSpeed(Character character) => extraArmor[level-1];
}
