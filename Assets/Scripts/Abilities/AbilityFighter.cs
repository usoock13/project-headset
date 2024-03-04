using System;
using UnityEngine;

public class AbilityFighter : Ability {
    [SerializeField] private Sprite icon;

    protected override (Sprite icon, string name, string description) InformationEN => (
        icon: this.icon,
        name: "Found Gold!",
        description: "<nobr>She takes extra damage, but gets additional ATTACK SPEED.</nobr>"
    );
    protected override (Sprite icon, string name, string description) InformationKO => (
        icon: this.icon,
        name: "활동복",
        description: "<nobr>받는 피해가 증가하지만 공격 속도도 함께 증가합니다.</nobr>"
    );

    public override void OnTaken(Character character) {
        character.extraArmor += GetDebuff;
        character.extraAttackSpeed += GetExtraAttackSpeed;
    }
    public override void OnReleased(Character character) {
        character.extraArmor -= GetDebuff;
        character.extraAttackSpeed -= GetExtraAttackSpeed;
    }

    private float GetExtraAttackSpeed(Character character) => 0.2f;
    public float GetDebuff(Character character) => -0.2f;
}