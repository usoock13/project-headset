using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAdventurer : Character {
    #region Animation Clips
    protected const string ANIMATION_IDLE = "Adventurer Idle";
    protected const string ANIMATION_WALK = "Adventurer Walk";
    #endregion Animation Clips

    #region Character Information
    public override string CharacterName => "모험가";
    #endregion Character Information

    protected override void InitializeStates() {
        base.InitializeStates();
        idleState.onActive = OnEnterIdleState;
        walkState.onActive = OnEnterWalkState;
    }
    private void OnEnterIdleState(State previous) {
        spriteAnimator.ChangeAnimation(ANIMATION_IDLE);
    }
    private void OnEnterWalkState(State previous) {
        spriteAnimator.ChangeAnimation(ANIMATION_WALK);
    }
}
