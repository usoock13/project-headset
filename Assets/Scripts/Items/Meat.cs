using System.Collections;
using UnityEngine;

public class Meat : Item {
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "고기";
    public override string Description => "즉시 캐릭터 체력을 30%를 회복시킵니다.";

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