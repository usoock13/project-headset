using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPriest : Character {
    #region Animation Clips
    const string ANIMATION_IDLE = "Priest Idle";
    const string ANIMATION_WALK = "Priest Walk";
    #endregion Animation Clips

    #region Character Information
    public override string CharacterName => "성직자";
    
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
