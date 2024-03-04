using UnityEngine;

public class SkillPriest : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;
    public override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "God Bless",
            Description: "Priest get the near-immortal POWER from omnipotent blessing of the god."
        );
    }
    public override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "신의 축복",
            Description: "성직자가 전지전능한 축복을 받아 거의 무적에 가까운 재생 능력을 얻습니다."
        );
    }
    #endregion Skill Information

    public float HealPerSecond => 0.1f * character.MaxHp + 15f;
    public Character Character => character;

    [SerializeField] private EffectBlessingArea blessingArea;
    private ObjectPooler AreaPooler;

    private void Awake() {
        AreaPooler = new ObjectPooler(
            poolingObject: blessingArea.gameObject,
            parent: this.transform,
            count: 2,
            restoreCount: 2
        );
    }
    public override void Active() {
        var area = AreaPooler.OutPool(transform.position, Quaternion.identity);
        area.GetComponent<EffectBlessingArea>()?.Active();
    }
}