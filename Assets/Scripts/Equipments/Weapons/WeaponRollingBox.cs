using System;
using System.Collections;
using UnityEngine;

public class WeaponRollingBox : Weapon {
    [SerializeField] private EffectBoxSlash rollingBoxEffect;
    private ObjectPooler effectPooler;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    protected override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] intervals = new float[MAX_LEVEL]    {  2f,    2f,   1.2f,    1.2f,   1.2f }; // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]   { 1f,   1.5f,   1.5f,      2f,   4.5f }; // 피해계수
    private float[] areaScale = new float[MAX_LEVEL]    {  1f,    1f,     1f,   1.25f,   1.5f }; // 공격 범위 축척
    protected override float AttackInterval => intervals[level];
    public float Damage => damageCoef[level];
    public float AreaScale => areaScale[level];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    [SerializeField] private string _weaponName;
    public override Sprite Icon {
        get { return _weaponIcon; }
    }
    public override string Name {
        get { return _weaponName; }
    }
    public override string Description {
        get {
            switch(level) {
                case 4 or 5:
                    return string.Join(Environment.NewLine,
                        $"{AttackInterval}초에 한 번 조준 방향으로 적을 관통하는 상자를 던져 적중하는 모든 적에게 {Damage*100}%의 피해를 가합니다.",
                        $"추가로 범위가 {(AreaScale-1) * 100}% 증가합니다.");
                default:
                    return
                        $"{AttackInterval}초에 한 번 조준 방향으로 상자를 던져 처음 적중하는 적 다섯에게 {Damage*100}%의 피해를 가합니다.";
            }
        }
    }
    protected override void Attack() {
        throw new NotImplementedException();
    }
    private IEnumerator InPoolEffect(float delay, GameObject effect) {
        yield return new WaitForSeconds(delay);
        effectPooler.InPool(effect);
    }
    #endregion Weapon Information
}