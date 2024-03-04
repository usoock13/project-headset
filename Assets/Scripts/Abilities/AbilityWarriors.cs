using System;
using UnityEngine;

public class AbilityWarriors : Ability {
    [SerializeField] private Sprite icon;

    protected override (Sprite icon, string name, string description) InformationEN => (
        icon: this.icon,
        name: "Tenacity",
        description: "<nobr>The less hp she has, the less damage she get.</nobr>"
    );
    protected override (Sprite icon, string name, string description) InformationKO => (
        icon: this.icon,
        name: "근성",
        description: "<nobr>체력이 적을수록 받는 피해가 감소합니다.</nobr>"
    );

    public override void OnTaken(Character character) {
        character.extraArmor += GetAdditionalArmor;
    }
    public override void OnReleased(Character character) {
        character.extraArmor -= GetAdditionalArmor;
    }
    private float GetAdditionalArmor (Character character) {
        float hpRatio = character.currentHp / character.MaxHp;
        return 20 * (1-hpRatio);
    }
}