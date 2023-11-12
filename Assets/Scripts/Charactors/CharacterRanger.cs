using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRanger : Character {
    #region Animation Clips
    const string ANIMATION_IDLE = "Ranger Idle";
    const string ANIMATION_WALK = "Ranger Walk";
    #endregion Animation Clips

    public override string CharacterName => "레인저";

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
