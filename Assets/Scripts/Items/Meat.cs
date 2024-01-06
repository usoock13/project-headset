using System.Collections;
using UnityEngine;

public class Meat : Item {
    [SerializeField] private Sprite _icon;

    protected override ItemInformation InformationEN => new ItemInformation(
        Icon: _icon,
        Name: "Meat",
        Description:
            "즉시 캐릭터 체력을 30%를 회복시킵니다."
    );
    protected override ItemInformation InformationKO => new ItemInformation(
        Icon: _icon,
        Name: "고기",
        Description:
            "즉시 캐릭터 체력을 30%를 회복시킵니다."
    );

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