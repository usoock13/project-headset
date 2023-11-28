using System;
using UnityEngine;

public class AbilityAlchemist : Ability {
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "황금 발견";
    public override string Description => "<nbr>케소를 획득하면 금액에 비례하여 경험치를 획득합니다.</nbr>";

    public override void OnTaken(Character character) {
        
    }
    public override void OnReleased(Character character) {
        
    }
    private float GetBonusExp(Item item) {
        throw new System.NotImplementedException();
    }
}