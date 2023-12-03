using System.Collections;
using UnityEngine;

public class WeaponSpirit : Weapon {
    [SerializeField] private EffectFireRing[] effects;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval         = new float[MAX_LEVEL] { 6.00f,  5.50f,  5.00f,  4.50f,  3.00f }; // 충전 시간
    private float[] runningTime      = new float[MAX_LEVEL] { 6.00f,  6.00f,  6.00f,  6.00f,  6.00f }; // 가동 시간
    private float[] staticDamage     = new float[MAX_LEVEL] {    8f,    10f,    12f,    14f,    16f }; // 고정 피해량
    private float[] damageCoef       = new float[MAX_LEVEL] { 0.15f,  0.20f,  0.25f,  0.30f,  0.35f }; // 피해 계수
    private int[] spiritCount        = new int[MAX_LEVEL]   {     1,      2,      2,      3,      3 }; // 정령수
    protected override float AttackInterval => interval[level-1];
    public float RunningTime                => runningTime[level-1];
    public float Damage                     => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    public int Count                        => spiritCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "정령";
    public override string Description =>
        (level+1) switch {
            _ => $"<nobr>적을 추격하며 공격하는 정령을 <color=#f40>{Count}마리</color> 소환합니다.\n"
               + $"정령은 <color=#f40>{runningTime[level]}초</color> 동안 비행하며 충돌한 적에게 "
               + $"<color=#f40>{staticDamage[level]}+{damageCoef[level] * 100}%</color>의 피해를 줍니다."
               + $"정령은 비행을 마치면 복귀해 <color=#f10>{interval[level]}초 동안 충전해야합니다.</color></nobr>"
        };
    #endregion Weapon Information
    public override void OnEquipped() {
        base.OnEquipped();
        int count = CurrentLevel<5 ? 3 : 6;
        for(int i=0; i<count; i++) {
            effects[i].gameObject.SetActive(true);
            effects[i].Active();
        }
    }

    protected override void OnLevelUp() {
        base.OnLevelUp();
        for( int i=0; i<(level>=5?6:3); i++ ) {
            effects[i].gameObject.SetActive(true);
            effects[i].Active();
        }
    }
    
    protected override void Attack() {}
}