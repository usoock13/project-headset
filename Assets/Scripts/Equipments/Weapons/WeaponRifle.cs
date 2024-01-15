using System;
using System.Collections;
using UnityEngine;

public class WeaponRifle : Weapon {
    [SerializeField] private EffectRifleBullet rifleEffect;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] staticDamage = new float[MAX_LEVEL]   {  25f,     55f,     85f,    115f,   145f }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]     { 0.4f,    0.5f,    0.6f,    0.7f,   0.8f }; // 피해 계수
    private int[] maxHitCount = new int[MAX_LEVEL]        {    3,       4,       5,       9,     99 }; // 최대 관통 횟수

    protected override float AttackInterval => 2.5f;
    public float HittingDelay => 1;
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public int MaxHitCount => maxHitCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Musket",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Shoot the piercing bullet to damage monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{AttackInterval}sec</color>"
                   + $"\nMax Hit Monsters : <color=#f40>{maxHitCount[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Shoot the piercing bullet to damage monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{AttackInterval}sec</color>"
                   + $"\nMax Hit Monsters : <color=#f40>{maxHitCount[level-1]}</color> > <color=#f40>{maxHitCount[NextLevelIndex]}</color>"
                   + $"</nobr>"
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "화승총",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"몬스터를 꿰뚫는 총알을 발사해 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"\n최대 공격 횟수 : <color=#f40>{maxHitCount[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"몬스터를 꿰뚫는 총알을 발사해 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"\n최대 공격 횟수 : <color=#f40>{maxHitCount[level-1]}</color> > <color=#f40>{maxHitCount[NextLevelIndex]}</color>"
                   + $"</nobr>"
            }
    );

    private void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: rifleEffect.gameObject,
            parent: this.transform
        );
    }
    protected override void Attack() {
        EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
    #endregion Weapon Information
}