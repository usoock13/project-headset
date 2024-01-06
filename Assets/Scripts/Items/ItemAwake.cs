using System.Collections;
using UnityEngine;

public class ItemAwake : Item {
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _itemNameEN;
    [SerializeField] private string _itemNameKO;

    protected override ItemInformation InformationEN => new ItemInformation(
        Icon: _icon,
        Name: _itemNameEN,
        Description: newWeapon.Description
    );
    protected override ItemInformation InformationKO => new ItemInformation(
        Icon: _icon,
        Name: _itemNameKO,
        Description: newWeapon.Description
    );

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
        GameManager.instance.StageManager.EquipmentsManager.RemoveEquipmentFromList(oldWeapon);
        GameManager.instance.StageManager.EquipmentsManager.RemoveBonusItemFromList(this);
    }
}