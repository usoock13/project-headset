using System.Collections;
using UnityEngine;

public class ItemAwake : Item {
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _itemName;

    public override Sprite Icon => _icon;
    public override string Name => _itemName;
    public override string Description => newWeapon.Description;

    [SerializeField] Weapon oldWeapon;
    [SerializeField] Weapon newWeapon;

    public override void Drop() {
        base.Drop();
    }
    public override void OnGotten() {
        base.OnGotten();
        GameManager.instance.StageManager.EquipmentsManager.AddEquipmentAtList(newWeapon);
        newWeapon.LevelUp();
        GameManager.instance.StageManager.EquipmentsManager.ChangeWeapon(oldWeapon, newWeapon);
        GameManager.instance.StageManager.EquipmentsManager.RemoveBonusItemFromList(this);
    }
}