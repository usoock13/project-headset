using UnityEngine;

public class SkillMage : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;
    protected override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "",
            Description: ""
        );
    }
    protected override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "",
            Description: ""
        );
    }
    #endregion Skill Information

    public override void Active() {
        
    }
}