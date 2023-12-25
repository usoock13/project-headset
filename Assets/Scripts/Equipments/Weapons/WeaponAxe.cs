using System.Collections;
using UnityEngine;

public class WeaponAxe : Weapon {
    [SerializeField] private EffectFlyingAxe projectile;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] interval = new float[MAX_LEVEL]         { 2.5f,    2.5f,    2.5f,    2.5f,    2.5f, }; // 공격 간격
    private float[] staticDamage = new float[MAX_LEVEL]     {  20f,     30f,     40f,     50f,     60f, }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]       { 0.5f,    0.6f,    0.7f,    0.8f,    0.9f, }; // 피해 계수
    private float[] projectileScale = new float[MAX_LEVEL]  { 1.0f,    1.0f,    1.5f,    1.5f,    2.0f, }; // 투사체 크기
    private int[] maxHitCount = new int[MAX_LEVEL]          {    4,       7,      10,      13,      99, }; // 최대 관통 횟수

    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float ProjectileScale => projectileScale[level-1];
    public int MaxHitCount => maxHitCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "벌목 도구";
    public override string Description =>
        NextLevelIndex switch {
            2 or 4  => $"<nobr><color=#f40>{interval[NextLevelIndex]}초</color>에 한 번 조준 방향을 향해 클래식 벌목 도구를 던져 적중하는 모든 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다. "
                     + $"벌목 도구의 크기가 <color=#f40>{(projectileScale[NextLevelIndex] - projectileScale[0]) * 100}%</color> 증가합니다.</nobr>",
            _       => $"<nobr><color=#f40>{interval[NextLevelIndex]}초</color>에 한 번 조준 방향을 향해 클래식 벌목 도구를 던져 적중하는 모든 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.</nobr>"
        };
    #endregion Weapon Information

    private void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: projectile.gameObject,
            parent: this.transform);
    }
    protected override void Attack() {
        EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
}