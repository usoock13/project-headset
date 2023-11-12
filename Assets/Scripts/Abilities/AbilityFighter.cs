using System;
using UnityEngine;

public class AbilityFighter : Ability
{
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "맨몸 주의";
    public override string Description => "<nbr>받는 피해가 증거하지만 공격 속도도 함께 증가합니다.</nbr>";

    public override void OnReleased(Character character) {
        throw new NotImplementedException();
    }
    public override void OnTaken(Character character) {
        throw new NotImplementedException();
    }
}