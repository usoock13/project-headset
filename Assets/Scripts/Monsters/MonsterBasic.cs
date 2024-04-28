using System;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class MonsterBasic : Monster {
    [SerializeField] private float defaultAttackPower = 10f;
    public float AttackPower => defaultAttackPower * (1 + (_StageManager.StageLevel - 1) * 0.5f);

    private bool canAttack = false;
    private float attackInterval = 1f;

    [SerializeField] private string monsterType;
    public override string MonsterType => monsterType;

    private const string ANIMATION_CHASE = "Chase";
    private const string ANIMATION_HIT = "Hit";
    private const string ANIMATION_DIE = "Die";

    private Coroutine dieCoroutine;

    public override void OnSpawn() {
        base.OnSpawn();
        canAttack = true;
    }

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
            if(other.gameObject.TryGetComponent(out IDamageable target))
                AttackTarget(target);
        }
    }
    private void AttackTarget(IDamageable character) {
        if(canAttack) {
            canAttack = false;
            character?.TakeDamage(AttackPower);
            StartCoroutine(AttackCoroutine());
        }
    }
    private IEnumerator AttackCoroutine() {
        yield return new WaitForSeconds(attackInterval);
        canAttack = true;
    }
    private IEnumerator DieCoroutine() {
        yield return new WaitForSeconds(3f);
        ownerPooler.InPool(this.gameObject);
    }
}