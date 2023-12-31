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
    private float[] freezingTime = new float[MAX_LEVEL]     {   0.6f,   0.7f,   0.8f,   0.8f,   1.0f,  }; // 빙결 시간
    private float[] areaScale = new float[MAX_LEVEL]        {     1f,     1f,     1f,   1.2f,   1.5f,  }; // 폭발 범위
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
                3 or 4 => $"<nobr>적에게 닿으면 폭발하여 좁은 범위의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고 <color=#f40>{freezingTime[NextLevelIndex]}초</color> 동안 빙결시킵니다.\n"
                        + $"기존의 {areaScale[NextLevelIndex] * 100}%의 피해 범위를 가집니다.</nobr>",
                _      => $"<nobr>적에게 닿으면 폭발하여 좁은 범위의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고 <color=#f40>{freezingTime[NextLevelIndex]}초</color> 동안 빙결시킵니다.</nobr>"
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "얼음창",
        Description: 
            NextLevelIndex switch {
                3 or 4 => $"<nobr>적에게 닿으면 폭발하여 좁은 범위의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고 <color=#f40>{freezingTime[NextLevelIndex]}초</color> 동안 빙결시킵니다.\n"
                        + $"기존의 {areaScale[NextLevelIndex] * 100}%의 피해 범위를 가집니다.</nobr>",
                _      => $"<nobr>적에게 닿으면 폭발하여 좁은 범위의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고 <color=#f40>{freezingTime[NextLevelIndex]}초</color> 동안 빙결시킵니다.</nobr>"
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
        var instance = EffectPooler.OutPool(transform.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
}