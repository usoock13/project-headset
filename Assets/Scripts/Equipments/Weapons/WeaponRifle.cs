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
    public override Sprite Icon => _weaponIcon;
    public override string Name => "출장형 천공기";
    public override string Description => 
        NextLevelIndex switch {
            _ => $"<nobr><color=#f40>{AttackInterval}</color>초에 한 번 조준 방향으로 관통하는 철갑탄을 발사해 적중한 모든 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.</nobr>",
        };

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