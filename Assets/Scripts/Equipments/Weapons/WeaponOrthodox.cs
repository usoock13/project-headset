using System;
using System.Collections;
using UnityEngine;

public class WeaponOrthodox : Weapon {
    [SerializeField] private EffectOnePunch onePunchEffect;
    [SerializeField] private EffectTwoPunch twoPunchEffect;
    [SerializeField] private EffectStraightPunch straightPunchEffect;
    private ObjectPooler onePunchEffectPooler;
    private ObjectPooler twoPunchEffectPooler;
    private ObjectPooler straightPunchEffectPooler;
    [SerializeField] float attackRange = .5f;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]                 {  1.3f,    1.2f,    1.1f,    1.0f,    0.9f }; // 공격 간격
    private float[] damageOfOneScale = new float[MAX_LEVEL]         {  0.4f,    0.5f,    0.6f,    0.7f,    0.8f }; // 원 펀치 피해계수
    private float[] damageOfTwoScale = new float[MAX_LEVEL]         { 0.55f,   0.65f,   0.75f,   0.85f,   0.95f }; // 투 펀치 피해계수
    private float[] damageOfStraightScale = new float[MAX_LEVEL]    {  1.5f,    1.5f,    1.5f,    1.5f,    1.5f }; // 스트레이트 피해계수
    protected override float AttackInterval => interval[level-1];
    public float DamageOfOne => damageOfOneScale[level-1] * Character.Power;
    public float DamageOfTwo => damageOfTwoScale[level-1] * Character.Power;
    public float DamageOfStraight => damageOfStraightScale[level-1] * Character.Power;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "오소독스";
    public override string Description => 
        level switch {
            4 => $"{interval[level]}초에 한 번 조준 방향을 향해 원 투 스트레이트 펀치를 날려 각각 {damageOfOneScale[level]*100}% / {damageOfTwoScale[level]*100}% / {damageOfStraightScale[level]*100}%의 피해를 가합니다.",
            _   => $"{interval[level]}초에 한 번 조준 방향을 향해 원 투 펀치를 날려 각각 {damageOfOneScale[level]*100}% / {damageOfTwoScale[level]*100}%의 피해를 가합니다.",
        };
    #endregion Weapon Information

    private void Awake() {
        onePunchEffectPooler = new ObjectPooler(onePunchEffect.gameObject, (GameObject instance) => {
            var effect = instance.GetComponent<EffectOnePunch>();
            effect.originWeapon = this;
        }, null, this.transform, 10, 5);
        twoPunchEffectPooler = new ObjectPooler(twoPunchEffect.gameObject, (GameObject instance) => {
            var effect = instance.GetComponent<EffectTwoPunch>();
            effect.originWeapon = this;
        }, null, this.transform, 10, 5);
        straightPunchEffectPooler = new ObjectPooler(straightPunchEffect.gameObject, (GameObject instance) => {
            var effect = instance.GetComponent<EffectStraightPunch>();
            effect.originWeapon = this;
        }, null, this.transform, 10, 5);
    }
    protected override void Attack() {
        StartCoroutine(AttackCoroutine());
        Character.OnAttack();
    }
    private IEnumerator AttackCoroutine() {
        Vector3 effectPoint = Character.attackArrow.position + Character.attackArrow.forward*attackRange;
        GameObject instance = onePunchEffectPooler.OutPool(effectPoint, Character.attackArrow.rotation);
        StartCoroutine(InPoolEffect(5f, instance, onePunchEffectPooler));

        yield return new WaitForSeconds(.15f);

        effectPoint = Character.attackArrow.position + Character.attackArrow.forward*attackRange;
        instance = twoPunchEffectPooler.OutPool(effectPoint, Character.attackArrow.rotation);
        StartCoroutine(InPoolEffect(5f, instance, twoPunchEffectPooler));

        if(level >= 5) {
            yield return new WaitForSeconds(.15f);
            effectPoint = Character.attackArrow.position + Character.attackArrow.forward*attackRange;
            instance = straightPunchEffectPooler.OutPool(effectPoint, Character.attackArrow.rotation);
            StartCoroutine(InPoolEffect(5f, instance, straightPunchEffectPooler));
        }
    }
    private IEnumerator InPoolEffect(float delay, GameObject effect, ObjectPooler pooler) {
        yield return new WaitForSeconds(delay);
        pooler.InPool(effect);
    }
}