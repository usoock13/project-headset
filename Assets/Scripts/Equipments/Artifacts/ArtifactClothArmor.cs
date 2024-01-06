using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactClothArmor : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] extraArmor = new float[] {  5f,   7f,   9f,   11f,   13f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Cloth Armor",
        Description: 
            $"<nobr>방어력이 <color=#f40>{extraArmor[NextLevelIndex]}</color>만큼 증가합니다.</nobr>"
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "천 갑옷",
        Description: 
            $"<nobr>방어력이 <color=#f40>{extraArmor[NextLevelIndex]}</color>만큼 증가합니다.</nobr>"
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
    private float GetExtraMoveSpeed(Character character) => character.DefaultMoveSpeed * extraArmor[level-1];
}
