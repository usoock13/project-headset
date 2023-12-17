using System.Collections;
using UnityEngine;

public class AWeaponMultiUseAxe : Weapon {
    [SerializeField] private EffectAwAxe projectile;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] staticDamage = new float[MAX_LEVEL] {   80f,     95f,    110f,    125f,    140f, }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]   { 0.40f,   0.40f,   0.40f,   0.40f,   0.40f, }; // 피해 계수

    protected override float AttackInterval => 2.5f;
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "절단용 부메랑";
    public override string Description =>
        NextLevelIndex switch {
            _       => $"<nobr><color=#f40>{AttackInterval}초</color>에 한 번 조준 방향을 향해 절단용 부메랑을 던져 범위 내의 적에게 <color=#f40>{damageCoef[NextLevelIndex]*100}%</color>의 피해를 매초 가합니다.</nobr>"
        };
    #endregion Weapon Information

    private void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: projectile.gameObject,
            parent: this.transform
        );
    }

    protected override void Attack() {
        EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
}