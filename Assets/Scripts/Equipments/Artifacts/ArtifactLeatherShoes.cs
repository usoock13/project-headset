using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactLeatherShoes : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraSpeed = new float[] {  0.20f,   0.30f,   0.40f,   0.50f,   0.60f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Leather Shoes",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"The character gets additional <color=#f40>Movement Speed</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraSpeed[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"The character gets additional <color=#f40>Movement Speed</color>."
                   + $"\n"
                   + $"\nAmount : <color=#f40>{extraSpeed[level-1]}</color> > <color=#f40>{extraSpeed[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "가죽 장화",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>이동 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraSpeed[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 추가 <color=#f40>이동 속도</color>를 얻습니다."
                   + $"\n"
                   + $"\n증가량 : <color=#f40>{extraSpeed[level-1]}</color> > <color=#f40>{extraSpeed[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraMoveSpeed += GetExtraMoveSpeed;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraMoveSpeed -= GetExtraMoveSpeed;
    }
    private float GetExtraMoveSpeed(Character character) => extraSpeed[level-1];
}
