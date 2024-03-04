using UnityEngine;

public class SkillMage : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;

    public override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "Final Spark",
            Description: "Mage opens a portal erupts light to burns monsters."
        );
    }
    public override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "최후의 섬광",
            Description: "마법사가 빛을 분출하는 포탈을 열어 몬스터를 불태웁니다."
        );
    }
    #endregion Skill Information

    public float DamagePerSecond => character.Power * 2.0f + 100f;
    [SerializeField] private EffectFinalPortal portalPrefab;
    public ObjectPooler PortalPooler { get; private set; }

    private void Awake() {
        PortalPooler = new ObjectPooler(
            poolingObject: portalPrefab.gameObject,
            parent: transform,
            count: 2,
            restoreCount: 2
        );
    }

    public override void Active() {
        var portal = PortalPooler.OutPool(transform.position + character.attackArrow.up + (Vector3.up * 0.5f), character.attackArrow.rotation);
        portal.GetComponent<EffectFinalPortal>()?.Active();
    }
}