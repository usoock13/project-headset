using Unity.VisualScripting;
using UnityEngine;

public class MonsterWolf : Monster {
    protected float attackPower = 10f;

    public override string MonsterType => "Monster Wolf";

    public override void TakeAttackDelay(float amount) {
        throw new System.NotImplementedException();
    }
    public override void TakeDamage(float amount) {
        currentHp -= amount;
        print(currentHp);
    }
    public override void TakeForce(Vector2 force) {
        throw new System.NotImplementedException();
    }

    protected override void InitializeStates() {
        #region Chase State
        chaseState.onActive += (State previous) => {};
        chaseState.onStay += () => {
            // movement.MoveToward(MoveVector * Time.deltaTime);
            movement.Translate(MoveVector * Time.deltaTime);
        };
        chaseState.onInactive += (State next) => {};
        #endregion Chase State

        #region Hit State
        hitState.onActive += (State previous) => {};
        hitState.onInactive += (State next) => {};
        #endregion Hit State

        #region Die State
        dieState.onActive += (State previous) => {
            stateMachine.isMuted = true;
        };
        dieState.onInactive += (State next) => {};
        #endregion Die State

        stateMachine.SetIntialState(chaseState);
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == TargetCharacter.tag) {
            var character = other.gameObject.GetComponent<Character>();
            HitChracter(character);
        }
    }
    private void HitChracter(Character character) {
        character?.TakeDamage(10f);
    }
}