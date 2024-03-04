using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAlchemist : Character {
    #region Animation Clips
    protected const string ANIMATION_IDLE = "Alchemist Idle";
    protected const string ANIMATION_WALK = "Alchemist Walk";
    #endregion Animation Clips

    #region Character Information
    public override string CharacterName { get {
        return GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => "Alchemist",
             "Korean (ko)" => "연금술사",

                         _ => "Alchemist",
        };
    }}
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
