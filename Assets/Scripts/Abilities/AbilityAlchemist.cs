using System;
using UnityEngine;

public class AbilityAlchemist : Ability {
    [SerializeField] private Sprite icon;

    protected override (Sprite icon, string name, string description) InformationEN => (
        icon: this.icon,
        name: "Found Gold!",
        description: "<nobr>When she gets the KESO, gets EXP also.</nobr>"
    );
    protected override (Sprite icon, string name, string description) InformationKO => (
        icon: this.icon,
        name: "황금 발견!",
        description: "<nobr>케소를 획득하면 금액에 비례하여 경험치를 획득합니다.</nobr>"
    );

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