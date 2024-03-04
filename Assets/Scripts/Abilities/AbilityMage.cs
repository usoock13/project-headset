using System;
using UnityEngine;

public class AbilityMage : Ability {
    [SerializeField] private Sprite icon;

    protected override (Sprite icon, string name, string description) InformationEN => (
        icon: this.icon,
        name: "Mana Management",
        description: "<nobr>Recovery of skill gauge increases.</nobr>"
    );
    protected override (Sprite icon, string name, string description) InformationKO => (
        icon: this.icon,
        name: "마나 운용",
        description: "<nobr>스킬 게이지 회복 속도가 증가합니다.</nobr>"
    );

    public override void OnTaken(Character character) {
        character.extraRecoveringSp += GetExtraRecoveringSP;
    }
    public override void OnReleased(Character character) {
        character.extraRecoveringSp -= GetExtraRecoveringSP;
    }
    private float GetExtraRecoveringSP (Character character) => 0.5f;
}