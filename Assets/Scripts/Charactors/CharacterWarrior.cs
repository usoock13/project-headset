using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWarrior : Character {
    #region Animation Clips
    const string ANIMATION_IDLE = "Warrior Idle";
    const string ANIMATION_WALK = "Warrior Walk";
    #endregion Animation Clips

    #region Character Information
    public override string CharacterName => "워리어";
    
    [SerializeField] private AbilityWarriors _headAiblity;
    public override Ability HeadAbility => _headAiblity;
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
