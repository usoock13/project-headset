using System;
using System.Collections;
using UnityEngine;

public class WeaponLightningDagger : Weapon {
    [SerializeField] private EffectLightningDagger effectOrigin;
    [SerializeField] private LineRenderer lineOrigin;
    public ObjectPooler EffectPooler { get; private set; }
    public ObjectPooler LineRendererPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]             {  1.5f,    1.5f,    1.0f,    1.0f,    1.0f };  // 공격 간격
    private float[] staticDamage = new float[MAX_LEVEL]         {   10f,     15f,     20f,     25f,     30f };  // 고정 피해량
    private float[] chainingStaticDamage = new float[MAX_LEVEL] {   30f,     40f,     50f,     60f,     70f };  // 고정 피해량 (연쇄)
    private float[] damageCoef = new float[MAX_LEVEL]           { 0.10f,   0.15f,   0.20f,   0.25f,   0.30f };  // 피해 계수
    private float[] chainingDamageCoef = new float[MAX_LEVEL]   { 0.55f,   0.55f,   0.55f,   0.55f,   0.55f };  // 피해 계수 (연쇄)
    private int[] chainingCount = new int[MAX_LEVEL]            {     3,       3,       5,       5,       8 };  // 피해 계수 (연쇄)
    
    protected override float AttackInterval => interval[level-1];
    public float Damage         => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float ChainingDamage => chainingDamageCoef[level-1] * _Character.Power + chainingStaticDamage[level-1];
    public int ChainingCount  => chainingCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Lightning Shuriken",
        Description:
            NextLevelIndex switch {
                _ => $"<nobr><color=#f40>{interval[NextLevelIndex]}초</color>에 한 번 조준 방향으로 고전압 표창을 던져 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.\n"
                + $"적중한 위치에서 주변 적을 향해 가정용 전압 수준의 전류가 뻗어 <color=#f40>{chainingStaticDamage[NextLevelIndex]}+{chainingDamageCoef[NextLevelIndex]*100}%</color>의 피해를 가하며, 최대 <color=#f40>{chainingCount[NextLevelIndex]}</color>의 적에게 연쇄됩니다.</nobr>"
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "뇌전수리검",
        Description:
            NextLevelIndex switch {
                _ => $"<nobr><color=#f40>{interval[NextLevelIndex]}초</color>에 한 번 조준 방향으로 고전압 표창을 던져 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.\n"
                + $"적중한 위치에서 주변 적을 향해 가정용 전압 수준의 전류가 뻗어 <color=#f40>{chainingStaticDamage[NextLevelIndex]}+{chainingDamageCoef[NextLevelIndex]*100}%</color>의 피해를 가하며, 최대 <color=#f40>{chainingCount[NextLevelIndex]}</color>의 적에게 연쇄됩니다.</nobr>"
            }
    );
    private void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: effectOrigin.gameObject,
            parent: this.transform
        );
        LineRendererPooler = new ObjectPooler(
            poolingObject: lineOrigin.gameObject,
            parent: this.transform
        );
    }
    protected override void Attack() {
        EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
    #endregion Weapon Information
}