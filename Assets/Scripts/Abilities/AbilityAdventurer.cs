using System;
using UnityEngine;

public class AbilityAdventurer : Ability {
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "경험주의";
    public override string Description => "<nbr>적을 공격하면 낮은 확률로 경험치 보석이 떨어집니다.\n"
                                        + "적에게 피해를 입으면 높은 확률로 경험치 보석이 떨어집니다.</nbr>";
 
    public override void OnTaken(Character character) {
        
    }
    public override void OnReleased(Character character) {
        
    }
}