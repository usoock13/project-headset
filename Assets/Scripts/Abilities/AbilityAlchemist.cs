using System;
using UnityEngine;

public class AbilityAlchemist : Ability {
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "황금 발견";
    public override string Description => "<nobr>케소를 획득하면 금액에 비례하여 경험치를 획득합니다.</nobr>";

    public override void OnTaken(Character character) {
        character.onGetItem += OnGetItem;
    }
    public override void OnReleased(Character character) {
        character.onGetItem -= OnGetItem;
    }
    private void OnGetItem(Item item) {
        if(item.TryGetComponent<Keso>(out var keso)) {
            GameManager.instance.Character.GetExp((int) (keso.Amount * 0.2f));
        }
    }
}