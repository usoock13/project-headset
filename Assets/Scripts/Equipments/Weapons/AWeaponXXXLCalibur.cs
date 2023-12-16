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

    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "쓰리엑스라지칼리버";
    public override string Description =>
        $"<nobr>XXXL급 범위로 주변을 크게 휘둘러 적중한 모든 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.\n"
      + $"이후 검을 좁은 범위에 내리쳐 <color=#f40>{secondStaticDamage[NextLevelIndex]}+{secondDamageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.</nobr>";

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
