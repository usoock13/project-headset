using System.Collections;
using UnityEngine;

public class Salad : Item {
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "샐러드";
    public override string Description => $"즉시 캐릭터의 스킬 게이지를 50% 회복시킵니다.";

    public override void OnGotten() {
        base.OnGotten();
        var target = GameManager.instance.Character;
        target.RecoverSkillGauge(target.MaxSp * 0.5f);
    }
}