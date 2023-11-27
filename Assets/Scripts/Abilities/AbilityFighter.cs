using System;
using UnityEngine;

public class AbilityFighter : Ability
{
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "활동복";
    public override string Description => "<nbr>받는 피해가 증거하지만 공격 속도도 함께 증가합니다.</nbr>";

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