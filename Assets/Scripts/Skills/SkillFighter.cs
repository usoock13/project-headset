using System.Collections;
using UnityEngine;

public class SkillFighter : Skill {
    #region Skill Information
    private bool isActive = false;
    [SerializeField] private Sprite _icon;
    protected override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "Break Limit",
            Description: "Fighter gets HUGE speed in exchange for to release half of HP."
        );
    }
    protected override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "한계 돌파",
            Description: "무투가가 체력 절반을 대가로 엄청난 속도를 얻습니다."
        );
    }
    #endregion Skill Information

    private float duration = 12f;
    private float hpRatioToLose = 0.5f;
    
    [SerializeField] private ParticleSystem particle;

    public override void Active() {
        if(isActive)
            Inactive();
        isActive = true;

        character.ConsumeHP(character.currentHp * hpRatioToLose);
        particle.Play();
        character.extraMoveSpeed += GetExtraMoveSpeed;
        character.extraAttackSpeed += GetExtraAttackSpeed;
        StartCoroutine( InactiveCoroutine() );
    }
    private void Inactive() {
        particle.Stop();
        character.extraMoveSpeed -= GetExtraMoveSpeed;
        character.extraAttackSpeed -= GetExtraAttackSpeed;
    }

    private IEnumerator InactiveCoroutine() {
        yield return new WaitForSeconds(duration);
        Inactive();
    }

    private float GetExtraMoveSpeed(Character c) => character.DefaultMoveSpeed;
    private float GetExtraAttackSpeed(Character c) => 1f;
}