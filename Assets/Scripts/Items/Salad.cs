using System.Collections;
using UnityEngine;

public class Salad : Item {
    [SerializeField] private Sprite _icon;
    
    protected override ItemInformation InformationEN => new ItemInformation(
        Icon: _icon,
        Name: "Salad",
        Description:
            $"Charge character's skill gauge 50%."
    );
    protected override ItemInformation InformationKO => new ItemInformation(
        Icon: _icon,
        Name: "샐러드",
        Description:
            $"즉시 캐릭터의 스킬 게이지를 50% 회복시킵니다."
    );

    public override void OnGotten() {
        base.OnGotten();
        var target = GameManager.instance.Character;
        target.RecoverSkillGauge(target.MaxSp * 0.5f);
    }
}