using System;
using System.Collections;
using UnityEngine;

public class WeaponRifle : Weapon {
    [SerializeField] private EffectRifleBullet rifleEffect;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] staticCoef = new float[MAX_LEVEL]   {   25f,     55f,     85f,    115f,   145f };  // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]   {  0.4f,    0.5f,    0.6f,    0.7f,   0.8f };  // 피해 계수
    private float[] hittingDelay = new float[MAX_LEVEL] {    1f,      1f,      1f,      2f,     2f };  // 경직 시간

    protected override float AttackInterval => 1;
    public float HittingDelay => hittingDelay[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "출장형 천공기";
    public override string Description => 
        level switch {
            _ => $"{AttackInterval}초에 한 번 조준 방향으로 관통하는 철갑탄을 발사해 적중한 모든 적에게 {damageCoef[level]*100}%의 피해를 가하고 {hittingDelay[level]}초 동안 경직시킵니다.",
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