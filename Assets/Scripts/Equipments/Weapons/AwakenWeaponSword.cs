using UnityEngine;

public class AwakenWeaponSword : Weapon {
    [SerializeField] private EffectAwakenSlash effect;
    public ObjectPooler EffectPooler { get; private set; }

    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] damageCoef = new float[MAX_LEVEL]   {    1f,    2f,    3f,    4f,    5f, };
    private float[] staticDamage = new float[MAX_LEVEL] {    1f,    2f,    3f,    4f,    5f, };
    private float[] areaScale = new float[MAX_LEVEL]    { 1.00f, 1.00f, 1.25f, 1.25f, 1.50f, };

    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float AreaScale => areaScale[level-1];
    protected override float AttackInterval => 1.5f;
    #endregion Weapon Status

    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "트리플엑스라지칼리버";
    public override string Description => $"<nobr>XXXL급 범위로 주변을 크게 휘둘러 적중한 모든 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.</nobr>";

    private void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: effect.gameObject,
            parent: this.transform,
            count: 5,
            restoreCount: 10
        );
    }
    protected override void Attack() {
        EffectPooler.OutPool(transform.position, _Character.attackArrow.rotation);
    }
}
