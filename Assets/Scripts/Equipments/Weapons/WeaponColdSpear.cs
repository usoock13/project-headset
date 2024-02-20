using System.Collections;
using UnityEngine;

public class WeaponColdSpear : Weapon {
    [SerializeField] private EffectColdSpear projectile;
    [SerializeField] private AttachmentFreeze attachment;
    [SerializeField] private GameObject sideEffect;
    public ObjectPooler EffectPooler { get; private set; }
    public ObjectPooler AttachmentPooler { get; private set; }
    public ObjectPooler SideEffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]         {   2.5f,   2.5f,   2.0f,   2.0f,   2.0f,  }; // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]       {   0.3f,   0.3f,   0.3f,   0.3f,   0.3f,  }; // 피해 계수
    private float[] staticDamage = new float[MAX_LEVEL]     {    15f,    15f,    25f,    25f,    30f,  }; // 고정 피해량
    private float[] freezingTime = new float[MAX_LEVEL]     {   0.8f,   1.1f,   1.1f,   1.4f,   1.6f,  }; // 빙결 시간
    // private float[] areaScale = new float[MAX_LEVEL]        {     1f,     1f,     1f,   1.2f,   1.5f,  }; // 폭발 범위
    protected override float AttackInterval => interval[level-1];

    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float FreezeTime => freezingTime[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    
    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Ice Spear",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Throw a ice spear to damage and freeze the monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"\nFreezing Time : <color=#f40>{freezingTime[0]}sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Throw a ice spear to damage and freeze the monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\nFreezing Time : <color=#f40>{freezingTime[level-1]}sec</color> > <color=#f40>{freezingTime[NextLevelIndex]}sec</color>"
                   + $"</nobr>"
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "얼음창",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"얼음 창을 던져 꿰뚫린 몬스터에게 피해를 가하고 얼립니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"\n빙결 지속시간 : <color=#f40>{freezingTime[0]}초</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"얼음 창을 던져 꿰뚫린 몬스터에게 피해를 가하고 얼립니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"\n빙결 지속시간 : <color=#f40>{freezingTime[level-1]}초</color> > <color=#f40>{freezingTime[NextLevelIndex]}초</color>"
                   + $"</nobr>"
            }
    );
    #endregion Weapon Information
    
    protected void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: projectile.gameObject,
            parent: this.transform
        );
        AttachmentPooler = new ObjectPooler(
            poolingObject: attachment.gameObject,
            parent: this.transform
        );
        SideEffectPooler = new ObjectPooler(
            poolingObject: sideEffect,
            parent: this.transform
        );
    }
    protected override void Attack() {
        EffectPooler.OutPool(transform.position + Vector3.up * 0.5f, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
}