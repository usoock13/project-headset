using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactLeatherShoes : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraSpeed = new float[] {  0.20f,   0.40f,   0.60f,   0.80f,   1.00f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Leather Shoes",
        Description:
            $"<nobr>이동속도가 <color=#f40>{extraSpeed[NextLevelIndex]}</color>만큼 증가합니다.</nobr>"
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "가죽 장화",
        Description:
            $"<nobr>이동속도가 <color=#f40>{extraSpeed[NextLevelIndex]}</color>만큼 증가합니다.</nobr>"
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
    private float GetExtraMoveSpeed(Character character) => character.DefaultMoveSpeed * extraSpeed[level-1];
}
