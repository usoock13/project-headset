using System;
using UnityEngine;

public class AbilityWarriors : Ability {
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "근성";
    public override string Description => "<nobr>체력이 적으면 받는 피해가 감소합니다.</nobr>";

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