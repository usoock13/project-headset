using UnityEngine;

public class SkillAdventurer : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;
    protected override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "Hallucination",
            Description: "Adventurer summons PERFECT hallucination that can't be distinguished from the real."
        );
    }
    protected override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "환영",
            Description: "모험가가 진짜화 구별할 수 없는 완벽한 환영을 소환합니다."
        );
    }
    #endregion Skill Information
    
    [SerializeField] private EffectHallucination hallu;
    [SerializeField] private float hallucinationHP = 1000f;

    public override void Active() {
        SummonHallucination();
    }

    private void SummonHallucination() {
        hallu.transform.position = transform.position;
        hallu.Active(hallucinationHP);
    }
}