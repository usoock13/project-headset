using System;
using System.Collections;
using UnityEngine;

public class WeaponWhip : Weapon {
    [SerializeField] private EffectWhip effect;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]     { 2.0f,   2.0f,   2.0f,   2.0f,   1.5f }; // 공격 간격
    private float[] staticDamage = new float[MAX_LEVEL] {  15f,    15f,    20f,    30f,    40f }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]   { 0.3f,   0.3f,   0.5f,   0.5f,   0.5f }; // 피해 계수
    private float[] hittingDelay = new float[MAX_LEVEL] { 0.5f,   0.6f,   0.6f,   0.7f,   0.7f }; // 경직 시간
    private float[] areaScale = new float[MAX_LEVEL]    { 1.0f,   1.0f,   1.0f,   1.0f,   1.0f }; // 공격 범위 축척
    protected override float AttackInterval => interval[level-1];
    
    public float Damage       => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float HittingDelay => hittingDelay[level-1];
    private float AreaScale    => areaScale[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Whip",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Swipe with the whip to damage monsters and gather hit monsters to center."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"</nobr>",
                4 => $"<nobr>"
                   + $"Swipe <color=#f40>twice</color> with the whip to damage monsters and gather hit monsters to center."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Swipe with the whip to damage monsters and gather hit monsters to center."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "채찍",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"채찍을 휘둘러 몬스터에게 피해를 주고 가운데로 끌어당깁니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"</nobr>",
                4 => $"<nobr>"
                   + $"채찍을 <color=#f40>두 번</color> 휘둘러 몬스터에게 피해를 주고 가운데로 끌어당깁니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"채찍을 휘둘러 몬스터에게 피해를 주고 가운데로 끌어당깁니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information

    private int attackDir = 1;

    private void Awake() {
        EffectPooler = new ObjectPooler(poolingObject: effect.gameObject, parent: this.transform, count: 5, restoreCount: 2);
    }
    protected override void Attack() {
        StartCoroutine(AttackCoroutine());
    }
    private IEnumerator AttackCoroutine() {
        GameObject instance;
        if(level < 5) {
            instance = EffectPooler.OutPool(transform.position, _Character.attackArrow.rotation);
            instance.transform.localScale = new Vector3(attackDir, 1, 1) * AreaScale;
            _Character.OnAttack();
            attackDir *= -1;
        } else {
            instance = EffectPooler.OutPool(transform.position, _Character.attackArrow.rotation);
            instance.transform.localScale = Vector3.one * AreaScale;
            _Character.OnAttack();
            yield return new WaitForSeconds(0.25f);
            instance = EffectPooler.OutPool(transform.position, _Character.attackArrow.rotation);
            instance.transform.localScale = new Vector3(-1, 1, 1) * AreaScale;
            _Character.OnAttack();
        }
    }
}