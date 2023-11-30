using System;
using System.Collections;
using UnityEngine;

public class MonsterWolf : Monster {
    protected float attackPower = 10f;

    public override string MonsterType => "Monster Wolf";

    private const string ANIMATION_CHASE = "Chase";
    private const string ANIMATION_HIT = "Hit";
    private const string ANIMATION_DIE = "Die";

    private Coroutine takeHittingDelayCoroutine;
    private Coroutine dieCoroutine;

    private float attackInterval = 1f;
    private float currentAttackCooldown = 1f;

    #region Unity Events
    private void Update() {
        if(currentAttackCooldown < attackInterval) {
            currentAttackCooldown += Time.deltaTime;
        }
    }
    #endregion Unity Events

    public override void TakeHittingDelay(float amount) {
        takeHittingDelayCoroutine = StartCoroutine(TakeAttackDelayCoroutine(amount));
    }
    private IEnumerator TakeAttackDelayCoroutine(float second) {
        stateMachine.ChangeState(hitState);
        yield return new WaitForSeconds(second);
        stateMachine.ChangeState(chaseState);
    }

    protected override void InitializeStates() {
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
        hitState.onInactive += (State next) => {
            StopCoroutine(takeHittingDelayCoroutine);
        };
        #endregion Hit State

        #region Die State
        dieState.onActive += (State previous) => {
            isArrive = false;
            stateMachine.isMuted = true;
            spriteAnimator.ChangeAnimation(ANIMATION_DIE);
            dieCoroutine = StartCoroutine(DieCoroutine());
            OnDie();
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