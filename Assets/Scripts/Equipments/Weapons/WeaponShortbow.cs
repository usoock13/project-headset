using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponShortbow : Weapon {
    [SerializeField] private EffectShortbow shortbowEffect;
    private ObjectPooler effectPooler;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]    {   .5f,     .5f,     .3f,      .3f,    .04f };  // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]   {  .45f,    .45f,    .45f,     .45f,    .45f };  // 피해계수
    private float[] hittingDelay = new float[MAX_LEVEL] {  .25f,    .25f,    .25f,     .25f,    .25f };  // 경직 시간
    private int[] arrowQuantity = new int[MAX_LEVEL]    {     1,       2,       2,        3,       1 };  // 투사체 수
    protected override float AttackInterval => interval[level-1];
    public float HittingDelay => hittingDelay[level-1];
    public float Damage => damageCoef[level-1] * Character.Power;
    public float ArrowQuantity => arrowQuantity[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "단궁";
    public override string Description => 
        level switch {
            _ => $"{interval[level]}초에 한 번 조준 방향으로 화살을 {arrowQuantity[level]}발 발사해 적중한 적에게 {damageCoef[level]*100}%의 피해를 가합니다.",
        };

    private void Awake() {
        effectPooler = new ObjectPooler(
            poolingObject: shortbowEffect.gameObject,
            onInPool: (gobj) => {
                // if(gobj.TryGetComponent<TrailRenderer>(out var tr))
                //     tr.enabled = false;
                //     tr.time = 0;
            },
            onOutPool: (gobj) => {
                // if(gobj.TryGetComponent<TrailRenderer>(out var tr)) {
                //     tr.enabled = true;
                //     tr.time = 0.12f;
                // }
            },
            onCreated: (gobj) => {
                var effect = gobj.GetComponent<EffectShortbow>();
                effect.originWeapon = this;
                effect.onDisapear += (projectile) => {
                    effectPooler.InPool(projectile.gameObject);
                };
            },
            this.transform, 100, 50
        );
    }
    protected override void Attack() {
        for(int i=0; i<ArrowQuantity; i++) {
            GameObject arrowInstance = effectPooler.OutPool(Character.attackArrow.position, Character.attackArrow.rotation);
            float aimJitter = UnityEngine.Random.Range(-7f, 7f);
            arrowInstance.transform.Rotate(Vector3.forward, aimJitter);
            Character.OnAttack();
        }
    }
    #endregion Weapon Information
}