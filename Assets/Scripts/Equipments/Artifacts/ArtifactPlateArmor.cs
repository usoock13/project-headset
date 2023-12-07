using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactPlateArmor : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] armorAmount = new float[]     {    12,      14,      16,      18,      20 };
    private float[] decreasingSpeed = new float[] { 0.30f,   0.25f,   0.20f,   0.15f,   0.10f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "판금 흉갑";
    public override string Description => 
        NextLevelIndex switch {
            _ => $"<nobr>방어력이 <color=#f40>{armorAmount[NextLevelIndex]}</color>만큼 증가하지만, 이동속도가 <color=#f40>{decreasingSpeed[NextLevelIndex] * 100}%</color>만큼 감소합니다.</nobr>"
        };
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.extraArmor += GetExtraArmor;
        _Character.extraMoveSpeed += GetExtraMoveSpeed;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.extraArmor -= GetExtraArmor;
        _Character.extraMoveSpeed -= GetExtraMoveSpeed;
    }
    private float GetExtraArmor(Character character) => armorAmount[level-1];
    private float GetExtraMoveSpeed(Character character) => character.DefaultMoveSpeed * -decreasingSpeed[level-1];
}
