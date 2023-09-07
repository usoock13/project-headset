using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorWarrior : Character {
    #region Animation Clips
    const string ANIMATION_IDLE = "Warrior Idle";
    const string ANIMATION_WALK = "Warrior Walk";
    #endregion Animation Clips
    
    protected override void InitializeStates() {
        base.InitializeStates();
        idleState.onActive = OnEnterIdleState;
        walkState.onActive = OnEnterWalkState;
    }
    private void OnEnterIdleState(State previous) {
        ChangeAnimation(ANIMATION_IDLE);
    }
    private void OnEnterWalkState(State previous) {
        ChangeAnimation(ANIMATION_WALK);
    }
}
