using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRanger : Character {
    #region Animation Clips
    const string ANIMATION_IDLE = "Ranger Idle";
    const string ANIMATION_WALK = "Ranger Walk";
    #endregion Animation Clips

    public HeadAbility headAbility = new HeadAbility {
        name = "기동전",
        description = "이동속도가 증가합니다.",
        onJoinParty = (Character character) => {
            character.additionalMoveSpeed += () => 0.5f;
        }
    };
    
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
