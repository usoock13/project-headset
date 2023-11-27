using System;
using UnityEngine;

public class AbilityMercenary : Ability {
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "두 개의 심장";
    public override string Description => "<nbr>스태미너 회복 속도가 증가합니다.</nbr>";

    public override void OnTaken(Character character) {
        character.extraRecoveringStamina += GetExtraRecoveringStamina;
    }
    public override void OnReleased(Character character) {
        character.extraRecoveringStamina -= GetExtraRecoveringStamina;
    }
    private float GetExtraRecoveringStamina (Character character) => 10f;
}