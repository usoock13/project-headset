using System.Collections.Generic;
using UnityEngine;

public class WeaponSpirit : Weapon {
    [SerializeField] private EffectSpirit[] spirits;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] chargeTime       = new float[MAX_LEVEL] { 6.00f,  5.00f,  4.00f,  3.00f,  2.00f }; // 충전 시간
    private float[] runningTime      = new float[MAX_LEVEL] { 6.00f,  6.00f,  6.00f,  6.00f,  6.00f }; // 가동 시간
    private float[] staticDamage     = new float[MAX_LEVEL] {    8f,    10f,    12f,    14f,    16f }; // 고정 피해량
    private float[] damageCoef       = new float[MAX_LEVEL] { 0.15f,  0.20f,  0.25f,  0.30f,  0.35f }; // 피해 계수
    // private float[] acceleration     = new float[MAX_LEVEL] { 1.00f,  1.00f,  2.00f,  2.00f,  3.00f }; // 가속도
    private int[] spiritCount        = new int[MAX_LEVEL]   {     1,      2,      2,      3,      3 }; // 정령 수

    protected override float AttackInterval => 0.5f;
    public float ChargeTime                 => chargeTime[level-1];
    public float RunningTime                => runningTime[level-1];
    public float Damage                     => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    public float Acceleration               => 3f;
    public int Count                        => spiritCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "무인 항공 정령";
    public override string Description =>
        (NextLevelIndex+1) switch {
            _ => $"<nobr>적을 추격하며 공격하는 정령을 <color=#f40>{spiritCount[NextLevelIndex]}마리</color> 소환합니다.\n"
               + $"정령은 <color=#f40>{runningTime[NextLevelIndex]}초</color> 동안 비행하며 충돌한 적에게 "
               + $"<color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가합니다."
               + $"정령은 비행을 마치면 복귀해 <color=#f10>{chargeTime[NextLevelIndex]}초</color> 동안 충전됩니다.</nobr>"
        };
    #endregion Weapon Information

    public Queue<EffectSpirit> Waitings { get; private set; } = new Queue<EffectSpirit>();
    public float attackCountdown = 0f;

    protected override void Update() {
        base.Update();
        if(Waitings.Count  >  0) {
            if(attackCountdown > 0)
                attackCountdown -= Time.deltaTime;
            else {
                Waitings.Dequeue().SearchMonster();
                attackCountdown += AttackInterval;
            }
        }
    }

    public override void OnEquipped() {
        base.OnEquipped();
        ActiveSpirits();
    }

    protected override void OnLevelUp() {
        base.OnLevelUp();
        ActiveSpirits();
    }

    public override void OnTakeOff() {
        base.OnTakeOff();
        
        Waitings.Clear();
        foreach(var spirit in spirits)
            spirit.gameObject.SetActive(false);
    }

    public void ActiveSpirits() {
        if(CurrentLevel == 0)
            return;
        int count = spiritCount[level-1];
        for(int i=0; i<count; i++) {
            if(!spirits[i].gameObject.activeInHierarchy) {
                spirits[i].gameObject.SetActive(true);
                Waitings.Enqueue(spirits[i]);
            }
        }
    }

    protected override void Attack() {}
}