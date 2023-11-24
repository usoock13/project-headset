using System.Collections;
using UnityEngine;

public class Potion : Item {
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "체력 회복 물약";
    public override string Description => "획득 즉시 캐릭터 최대 체력의 30%를 회복합니다.";

    public override void Drop() {
        base.Drop();
    }
    public override void OnGotten() {
        base.OnGotten();
        var character = GameManager.instance.Character;
        float healAmount = character.MaxHp * .3f;
        character.TakeHeal(healAmount);
    }
}