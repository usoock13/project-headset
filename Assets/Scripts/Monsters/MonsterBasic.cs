using System;
using System.Collections;
using UnityEngine;

public class MonsterBasic : Monster {
    [SerializeField] private float attackPower = 10f;

    private float attackInterval = 1f;
    private float currentAttackCooldown = 1f;

    [SerializeField] private string monsterType;
    public override string MonsterType => monsterType;

    private const string ANIMATION_CHASE = "Chase";
    private const string ANIMATION_HIT = "Hit";
    private const string ANIMATION_DIE = "Die";

    private Coroutine dieCoroutine;

    #region Unity Events
    private void Update() {
        if(currentAttackCooldown < attackInterval) {
            currentAttackCooldown += Time.deltaTime;
        }
    }
    #endregion Unity Events

    protected override void InitializeStates() {
        base.InitializeStates();
        #region Chase State
        chaseState.onActive += (State previous) => {};
        chaseState.onStay += () => {
            spriteAnimator.ChangeAnimation(ANIMATION_CHASE);
            movement.Translate(MoveVector * Time.deltaTime);
            LookAt2D(targetDirection.x);
        };
        chaseState.onInactive += (State next) => {};
        #endregion Chase State

        #region Hit State
        hitState.onActive += (State previous) => {
            spriteAnimator.ChangeAnimation(ANIMATION_HIT);
        };
        #endregion Hit State

        #region Die State
        dieState.onActive += (State previous) => {
            spriteAnimator.ChangeAnimation(ANIMATION_DIE);
            dieCoroutine = StartCoroutine(DieCoroutine());
        };
        dieState.onInactive += (State next) => {
            StopCoroutine(dieCoroutine);
        };
        #endregion Die State

        stateMachine.SetIntialState(chaseState);
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == TargetCharacter.tag) {
            var character = other.gameObject.GetComponent<Character>();
            HitChracter(character);
        }
    }
    private void HitChracter(Character character) {
        if(currentAttackCooldown >= attackInterval) {
            currentAttackCooldown = 0;
            character?.TakeAttack(this, 10f);
        }
    }
    private IEnumerator DieCoroutine() {
        yield return new WaitForSeconds(3f);
        ownerPooler.InPool(this.gameObject);
    }
}