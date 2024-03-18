using System;
using System.Collections;
using UnityEngine;

public class WeaponLightningDagger : Weapon {
    [SerializeField] private EffectLightningDagger effectOrigin;
    [SerializeField] private LineRenderer lineOrigin;
    public ObjectPooler EffectPooler { get; private set; }
    public ObjectPooler LineRendererPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]             {  1.5f,    1.5f,    1.0f,    1.0f,    1.0f };  // 공격 간격
    private float[] staticDamage = new float[MAX_LEVEL]         {   10f,     15f,     20f,     25f,     30f };  // 고정 피해량
    private float[] chainingStaticDamage = new float[MAX_LEVEL] {   30f,     40f,     50f,     60f,     70f };  // 고정 피해량 (연쇄)
    private float[] damageCoef = new float[MAX_LEVEL]           { 0.10f,   0.15f,   0.20f,   0.25f,   0.30f };  // 피해 계수
    private float[] chainingDamageCoef = new float[MAX_LEVEL]   { 0.55f,   0.55f,   0.55f,   0.55f,   0.55f };  // 피해 계수 (연쇄)
    private int[] chainingCount = new int[MAX_LEVEL]            {     3,       3,       5,       5,       8 };  // 피해 계수 (연쇄)
    
    protected override float AttackInterval => interval[level-1];
    public float Damage         => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float ChainingDamage => chainingDamageCoef[level-1] * _Character.Power + chainingStaticDamage[level-1];
    public int ChainingCount  => chainingCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Lightning Shuriken",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Throw a dagger with lightning. When the monster gets hit, lightning strike generates and bounces to other monster around the hit monster."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nLightning Damage : <color=#f40>{chainingStaticDamage[0]}+{chainingDamageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"\nNumber of Bouncing : <color=#f40>{chainingCount[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Throw a dagger with lightning. When the monster gets hit, lightning strike generates and bounces to other monster around the hit monster."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nDamage : <color=#f40>{chainingStaticDamage[level-1]}+{chainingDamageCoef[level-1]*100}%</color> > <color=#f40>{chainingStaticDamage[NextLevelIndex]}+{chainingDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\nNumber of Bouncing : <color=#f40>{chainingCount[level-1]}</color> > <color=#f40>{chainingCount[NextLevelIndex]}</color>"
                   + $"</nobr>"
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "뇌전수리검",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"번개가 흐르는 단검을 던집니다. 몬스터가 단검에 맞으면 번개가 발생해 주변 몬스터에게 튕기며 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n번개 피해량 : <color=#f40>{chainingStaticDamage[0]}+{chainingDamageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"\n튕기는 횟수 : <color=#f40>{chainingCount[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"번개가 흐르는 단검을 던집니다. 몬스터가 단검에 맞으면 번개가 발생해 주변 몬스터에게 튕기며 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n번개 피해량 : <color=#f40>{chainingStaticDamage[level-1]}+{chainingDamageCoef[level-1]*100}%</color> > <color=#f40>{chainingStaticDamage[NextLevelIndex]}+{chainingDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"\n튕기는 횟수 : <color=#f40>{chainingCount[level-1]}</color> > <color=#f40>{chainingCount[NextLevelIndex]}</color>"
                   + $"</nobr>"
            }
    );
    private void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: effectOrigin.gameObject,
            parent: this.transform
        );
        LineRendererPooler = new ObjectPooler(
            poolingObject: lineOrigin.gameObject,
            parent: this.transform
        );
    }
    protected override void Attack() {
        EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
    #endregion Weapon Information
}