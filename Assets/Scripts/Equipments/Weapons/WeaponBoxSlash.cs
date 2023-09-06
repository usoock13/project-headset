using System.Collections;
using UnityEngine;

public class WeaponBoxSlash : Weapon {
    private Character character;
    [SerializeField] private GameObject boxEffect;
    private ObjectPooler effectPooler;

    private void Awake() {
        character = GameManager.instance.Character;
        effectPooler = new ObjectPooler(boxEffect, null, null, this.transform, 10, 5);
    }

    protected override void Attack() {
        var effect = effectPooler.OutPool(character.attackArrow.position, character.attackArrow.rotation);
        StartCoroutine(InPoolEffect(5f, effect));
    }
    private IEnumerator InPoolEffect(float delay, GameObject effect) {
        yield return new WaitForSeconds(delay);
        effectPooler.InPool(effect);
    }
}