using UnityEngine;

public class WeaponBoxSlash : Weapon {
    private Character character;
    [SerializeField] private GameObject boxEffect;

    private void Awake() {
        character = GameManager.instance.Character;
    }

    protected override void Attack() {
        GameObject effect = Instantiate(boxEffect, character.attackDirection.position, character.attackDirection.rotation);
    }
}