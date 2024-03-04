using System;
using UnityEngine;

public class AbilityAdventurer : Ability {
    [SerializeField] private Sprite icon;

    protected override (Sprite icon, string name, string description) InformationEN => (
        icon: this.icon,
        name: "Adventurous",
        description: "<nobr>When she takes damage or deals damage to monster, EXP jewels drops.</nobr>"
    );
    protected override (Sprite icon, string name, string description) InformationKO => (
        icon: this.icon,
        name: "모험심",
        description: "<nobr>몬스터를 공격하거나 피해를 입으면 일정 확률로 경험치 보석이 떨어집니다.</nobr>"
    );

    private const int EXP_AMOUNT = 10;
 
    public override void OnTaken(Character character) {
        character.onAttackMonster += OnAttackMonster;
        character.onTakeAttack += OnTakeAttack;
    }
    public override void OnReleased(Character character) {
        character.onAttackMonster -= OnAttackMonster;
        character.onTakeAttack += OnTakeAttack;
    }
    private void OnAttackMonster(Character character, Monster target) {
        float probability = UnityEngine.Random.Range(0, 100);
        if(probability <= 2) {
            GameManager.instance.StageManager.CreateExp(character.transform.position, EXP_AMOUNT);
        }
    }
    private void OnTakeAttack(Character character, Monster origin, float amount) {
        float probability = UnityEngine.Random.Range(0, 100);
        if(probability <= 20) {
            GameManager.instance.StageManager.CreateExp(origin.transform.position, EXP_AMOUNT);
            GameManager.instance.StageManager.CreateExp(origin.transform.position, EXP_AMOUNT);
            GameManager.instance.StageManager.CreateExp(origin.transform.position, EXP_AMOUNT);
            GameManager.instance.StageManager.CreateExp(origin.transform.position, EXP_AMOUNT);
            GameManager.instance.StageManager.CreateExp(origin.transform.position, EXP_AMOUNT);
            GameManager.instance.StageManager.CreateExp(origin.transform.position, EXP_AMOUNT);
            GameManager.instance.StageManager.CreateExp(origin.transform.position, EXP_AMOUNT);
        }
    }
}