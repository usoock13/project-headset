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
    private float[] staticDamage = new float[MAX_LEVEL] {  15f,    15f,    20f,    20f,    20f }; // 고정 피해량
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
    public override Sprite Icon         => _weaponIcon;
    public override string Name         => "포획용 채찍";
    public override string Description  =>
        (NextLevelIndex+1) switch {
            5 => $"<nobr><color=#f40>{interval[NextLevelIndex]}초</color>에 한 번 조준 방향을 향해 채찍을 휘둘러 범위 내의 적에게 <color=#f40>{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가하고 휘두른 방향으로 몰아 넣습니다. 가까이 있는 적은 공격에 맞지 않습니다.</nobr>",
            _ => $"<nobr><color=#f40>{interval[NextLevelIndex]}초</color>에 한 번 조준 방향을 향해 채찍을 두 번 휘둘러 범위 내의 적에게 <color=#f40>{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가하고 가운데로 몰아 넣습니다. 가까이 있는 적은 공격에 맞지 않습니다.</nobr>"
        };
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