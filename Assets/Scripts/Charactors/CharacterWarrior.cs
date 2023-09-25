using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWarrior : Character {
    #region Animation Clips
    const string ANIMATION_IDLE = "Warrior Idle";
    const string ANIMATION_WALK = "Warrior Walk";
    #endregion Animation Clips

    public HeadAbility headAbility = new HeadAbility {
        name = "인내",
        description = "체력이 적으면 받는 피해량이 감소합니다.",
        onJoinParty = (Character character) => {
            character.additionalArmor += () => {
                float hpRatio = character.currentHp / character.MaxHp;
                return 15 * (1-hpRatio);
            };
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
