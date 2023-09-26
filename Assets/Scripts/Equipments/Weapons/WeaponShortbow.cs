using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponShortbow : Weapon {
    [SerializeField] private EffectShortbow shortbowEffect;
    private ObjectPooler effectPooler;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    protected override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] intervals = new float[MAX_LEVEL]    {   .5f,     .5f,     .3f,      .3f,    .04f };  // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]   {   .3f,     .3f,     .4f,      .4f,     .4f };  // 피해계수
    private float[] hittingDelay = new float[MAX_LEVEL] {  .25f,    .25f,    .25f,     .25f,    .25f };  // 경직 시간
    private int[] arrowQuantity = new int[MAX_LEVEL]    {     1,       2,       2,        3,       1 };  // 투사체 수
    protected override float AttackInterval => intervals[level];
    public float HittingDelay => hittingDelay[level];
    public float Damage => damageCoef[level] * Character.Power;
    public float ArrowQuantity => arrowQuantity[level];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "단궁";
    public override string Description => 
        level switch {
            _ => $"{AttackInterval}초에 한 번 조준 방향으로 화살을 {ArrowQuantity}발 발사해 관통하는 모든 적에게 {Damage*100}%의 피해를 가하고 {HittingDelay}초 동안 경직시킵니다.",
        };

    private void Awake() {
        effectPooler = new ObjectPooler(shortbowEffect.gameObject, null, null,
        (gobj) => {
            gobj.GetComponent<EffectShortbow>().onDisapear += (projectile) => {
                effectPooler.InPool(projectile.gameObject);
            };
        },
        this.transform, 100, 50);
    }
    protected override void Attack() {
        for(int i=0; i<ArrowQuantity; i++) {
            GameObject arrowInstance = effectPooler.OutPool(Character.attackArrow.position, Character.attackArrow.rotation);
            var effect = arrowInstance.GetComponent<EffectShortbow>();
            effect.originWeapon = this;

            float aimJitter = UnityEngine.Random.Range(-7f, 7f);
            arrowInstance.transform.Rotate(Vector3.forward, aimJitter);
        }
    }
    #endregion Weapon Information
}