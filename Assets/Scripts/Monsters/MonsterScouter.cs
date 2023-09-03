using UnityEngine;

public class MonsterScouter : Monster {
    public override void ChaseCharacter(Character target) {
        targetCharacter = target;
        stateMachine.ChangeState(chaseState);
    }

    public override void TakeAttackDelay(float amount) {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage(float amount) {
        print($"Damaged as much as {amount}");
    }

    public override void TakeForce(Vector2 force) {
        throw new System.NotImplementedException();
    }

    protected void InitializeState() {
        chaseState.onActive += (State previous) => {};
        chaseState.onActive += (State previous) => {
            movement.MoveToward(MoveVector * Time.deltaTime);
        };
        chaseState.onInactive += (State next) => {};
        hidState.onActive += (State previous) => {};
        hidState.onInactive += (State next) => {};
        dieState.onActive += (State previous) => {
            stateMachine.isMuted = true;
        };
        dieState.onInactive += (State next) => {};
    }
}