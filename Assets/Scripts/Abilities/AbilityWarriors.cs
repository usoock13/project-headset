using System;
using UnityEngine;

public class AbilityWarriors : Ability {
    private Sprite icon;

    public override Sprite Icon => icon;
    public override string Name => "인내";
    public override string Description => "<nbr>체력이 적으면 받는 피해가 감소합니다.</nbr>";

    public override void OnTaken(Character character) {
        character.additionalArmor += GetAdditionalArmor;
    }
    public override void OnReleased(Character character) {
        character.additionalArmor -= GetAdditionalArmor;
    }
    private float GetAdditionalArmor (Character character) {
        float hpRatio = character.currentHp / character.MaxHp;
        return 15 * (1-hpRatio);
    }
}