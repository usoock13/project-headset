using System;
using System.Collections;
using UnityEngine;

public class WeaponRifle : Weapon {
    [SerializeField] private EffectRifleBullet rifleEffect;
    private ObjectPooler effectPooler;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]    {  1f,      1f,    1f,      1f,    1f };  // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]   {  2f,    2.5f,    3f,    3.5f,    3f };  // 피해계수
    private float[] hittingDelay = new float[MAX_LEVEL] {  1f,      1f,    1f,      2f,    2f };  // 경직 시간
    private int[] arrowQuantity = new int[MAX_LEVEL]    {  1,        1,     1,       1,     3 };  // 투사체 수
    protected override float AttackInterval => interval[level-1];
    public float HittingDelay => hittingDelay[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power;
    public float ArrowQuantity => arrowQuantity[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "철갑상어탄";
    public override string Description => 
        level switch {
            _ => $"{interval[level]}초에 한 번 조준 방향으로 관통하는 철갑탄을 발사해 적중한 모든 적에게 {damageCoef[level]*100}%의 피해를 가하고 {hittingDelay[level]}초 동안 경직시킵니다.",
        };

    private void Awake() {
        effectPooler = new ObjectPooler(
            poolingObject: rifleEffect.gameObject,
            onCreated: (gobj) => {
                var effect = gobj.GetComponent<EffectRifleBullet>();
                effect.originWeapon = this;
                effect.onDisapear += (projectile) => {
                    effectPooler.InPool(projectile.gameObject);
                };
            },
            parent: this.transform
        );
    }
    protected override void Attack() {
        GameObject bulletInstance = effectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
    #endregion Weapon Information
}