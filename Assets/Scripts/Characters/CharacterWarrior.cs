using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWarrior : Character {
    #region Animation Clips
    const string ANIMATION_IDLE = "Warrior Idle";
    const string ANIMATION_WALK = "Warrior Walk";
    #endregion Animation Clips

    #region Character Information
    public override string CharacterName { get {
        return GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => "Warrior",
             "Korean (ko)" => "전사",

                         _ => "Warrior",
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
