using System;
using UnityEngine;

public class AbilityMage : Ability {
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "마나 처리 운용 기능사";
    public override string Description => "<nobr>스킬 게이지 회복 속도가 증가합니다.</nobr>";

    public override void OnTaken(Character character) {
        character.extraRecoveringSp += GetExtraRecoveringSP;
    }
    public override void OnReleased(Character character) {
        character.extraRecoveringSp -= GetExtraRecoveringSP;
    }
    private float GetExtraRecoveringSP (Character character) => 0.5f;
}