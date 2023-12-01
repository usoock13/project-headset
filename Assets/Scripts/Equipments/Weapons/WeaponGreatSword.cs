using System;
using System.Collections;
using UnityEngine;

public class WeaponGreatSword : Weapon {
    [SerializeField] private EffectGreatSword swordEffect;
    private ObjectPooler effectPooler;
    [SerializeField] private float attackRange = .5f;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]     {  2f,     2f,   1.2f,    1.2f,   1.2f }; // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]   {  1f,   1.5f,   1.5f,      2f,   4.5f }; // 피해계수
    private float[] areaScale = new float[MAX_LEVEL]    {  1f,     1f,     1f,   1.25f,   1.5f }; // 공격 범위 축척
    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power;
    public float AreaScale => areaScale[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "그레이트 소드";
    public override string Description {
        get {
            switch(level) {
                default:
                    return
                        $"{interval[level]}초에 한 번 조준 방향을 향해 대검을 휘둘러 적중한 적에게 {damageCoef[level]*100}%의 피해를 가합니다.";
                case 3 or 4:
                    return string.Join(Environment.NewLine,
                        $"{interval[level]}초에 한 번 조준 방향을 향해 대검을 휘둘러 적중한 적에게 {damageCoef[level]*100}%의 피해를 가합니다.",
                        $"추가로 범위가 {(areaScale[level]-1) * 100}% 증가합니다.");
            }
        }
    }
    #endregion Weapon Information

    private void Awake() {
        effectPooler = new ObjectPooler(poolingObject: swordEffect.gameObject, parent: this.transform);
    }
    protected override void Attack() {
        Vector3 effectPoint = _Character.attackArrow.position + _Character.attackArrow.forward*attackRange;
        GameObject instance = effectPooler.OutPool(effectPoint, _Character.attackArrow.rotation);
        StartCoroutine(InPoolEffect(5f, instance));
        _Character.OnAttack();
    }
    private IEnumerator InPoolEffect(float delay, GameObject effect) {
        yield return new WaitForSeconds(delay);
        effectPooler.InPool(effect);
    }
}