using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMage : Character {
    #region Animation Clips
    const string ANIMATION_IDLE = "Mage Idle";
    const string ANIMATION_WALK = "Mage Walk";
    #endregion Animation Clips

    #region Character Information
    public override string CharacterName { get {
        return GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => "Mage",
             "Korean (ko)" => "마법사",

                         _ => "Mage",
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
