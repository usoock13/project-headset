using System.Collections;
using UnityEngine;

public class AWeaponXXXLCalibur : Weapon {
    [SerializeField] private EffectXXXLSlash slashEffect;
    [SerializeField] private EffectXXXLStrike strikeEffect;
    public ObjectPooler FirstEffectPooler { get; private set; }
    public ObjectPooler SecondEffectPooler { get; private set; }

    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] damageCoef = new float[MAX_LEVEL]         {  1.5f,    1.5f,    1.5f,    1.5f,    1.5f, };
    private float[] staticDamage = new float[MAX_LEVEL]       {  050f,    070f,    090f,    110f,    130f, };
    private float[] secondDamageCoef = new float[MAX_LEVEL]   {    3f,      3f,      3f,      3f,      3f, };
    private float[] secondStaticDamage = new float[MAX_LEVEL] {  150f,    200f,    250f,    350f,    450f, };
    private float[] areaScale = new float[MAX_LEVEL]          { 1.00f,   1.00f,   1.25f,   1.25f,   1.50f, };

    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float SecondDamage => secondDamageCoef[level-1] * _Character.Power + secondStaticDamage[level-1];
    public float AreaScale => areaScale[level-1];
    protected override float AttackInterval => 1.5f;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "The Inquisitor",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Swing a giant sword horizontally and vertically to damage great."
                   + $"\n"
                   + $"\nHorizontal Damage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nVertical Damage : <color=#f40>{secondStaticDamage[0]}+{secondDamageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{AttackInterval}sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Swing a giant sword horizontally and vertically to damage great."
                   + $"\n"
                   + $"\nHorizontal Damage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nVertical Damage : <color=#f40>{secondStaticDamage[level-1]}+{secondDamageCoef[level-1]*100}%</color> > <color=#f40>{secondStaticDamage[NextLevelIndex]}+{secondDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{AttackInterval}sec</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "이단심판관",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"거대한 검을 가로세로로 휘둘러 큰 피해를 가합니다."
                   + $"\n"
                   + $"\n가로 피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n세로 피해량 : <color=#f40>{secondStaticDamage[0]}+{secondDamageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"거대한 검을 가로세로로 휘둘러 큰 피해를 가합니다."
                   + $"\n"
                   + $"\n가로 피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n세로 피해량 : <color=#f40>{secondStaticDamage[level-1]}+{secondDamageCoef[level-1]*100}%</color> > <color=#f40>{secondStaticDamage[NextLevelIndex]}+{secondDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information

    private void Awake() {
        FirstEffectPooler = new ObjectPooler(
            poolingObject: slashEffect.gameObject,
            parent: this.transform,
            count: 5,
            restoreCount: 10
        );
        SecondEffectPooler = new ObjectPooler(
            poolingObject: strikeEffect.gameObject,
            parent: this.transform,
            count: 5,
            restoreCount: 10
        );
    }

    protected override void Attack() {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        var slash = FirstEffectPooler.OutPool(transform.position, _Character.attackArrow.rotation);
        // slash.transform.localScale = Vector3.one * AreaScale;
        slash.GetComponent<EffectXXXLSlash>()?.Active();

        yield return new WaitForSeconds(0.25f);
        
        var strike = SecondEffectPooler.OutPool(transform.position, _Character.attackArrow.rotation);
        strike.transform.localScale = Vector3.one * AreaScale;
        strike.GetComponent<EffectXXXLStrike>()?.Active();
    }

    public override void OnTakeOff() {
        base.OnTakeOff();
        StopAllCoroutines();
    }
}
