using System;
using UnityEngine;

public class AbilityMercenary : Ability {
    [SerializeField] private Sprite icon;

    protected override (Sprite icon, string name, string description) InformationEN => (
        icon: this.icon,
        name: "Double Hearts",
        description: "<nobr>Recovery of stamina gauge increases.</nobr>"
    );
    protected override (Sprite icon, string name, string description) InformationKO => (
        icon: this.icon,
        name: "두 개의 심장",
        description: "<nobr>스태미너 회복 속도가 증가합니다.</nobr>"
    );

    public override void OnTaken(Character character) {
        character.extraRecoveringStamina += GetExtraRecoveringStamina;
    }
    public override void OnReleased(Character character) {
        character.extraRecoveringStamina -= GetExtraRecoveringStamina;
    }
    private float GetExtraRecoveringStamina (Character character) => 10f;
}