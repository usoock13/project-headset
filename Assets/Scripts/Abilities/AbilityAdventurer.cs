using System;
using UnityEngine;

public class AbilityAdventurer : Ability {
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "모험심";
    public override string Description => "<nbr>적을 공격하면 낮은 확률로 경험치 보석이 떨어집니다.\n"
                                        + "적에게 피해를 입으면 보다 높은 확률로 경험치 보석이 떨어집니다.</nbr>";
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
        }
    }
}