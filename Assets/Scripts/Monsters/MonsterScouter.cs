using UnityEngine;

public class MonsterScouter : Monster {
    public override string MonsterType => throw new System.NotImplementedException();

    protected override void InitializeStates() {
        chaseState.onActive += (State previous) => {};
        chaseState.onActive += (State previous) => {
            movement.MoveToward(MoveVector * Time.deltaTime);
        };
        chaseState.onInactive += (State next) => {};
        hitState.onActive += (State previous) => {};
        hitState.onInactive += (State next) => {};
        dieState.onActive += (State previous) => {
            stateMachine.isMuted = true;
        };
        dieState.onInactive += (State next) => {};
    }
}