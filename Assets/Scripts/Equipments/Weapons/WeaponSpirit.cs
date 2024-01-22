using System.Collections.Generic;
using UnityEngine;

public class WeaponSpirit : Weapon {
    [SerializeField] private EffectSpirit[] spirits;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] chargeTime       = new float[MAX_LEVEL] { 6.00f,  5.00f,  4.00f,  3.00f,  2.00f }; // 충전 시간
    private float[] runningTime      = new float[MAX_LEVEL] { 6.00f,  7.00f,  8.00f,  9.00f, 10.00f }; // 가동 시간
    private float[] staticDamage     = new float[MAX_LEVEL] {    8f,    10f,    12f,    14f,    16f }; // 고정 피해량
    private float[] damageCoef       = new float[MAX_LEVEL] { 0.15f,  0.20f,  0.25f,  0.30f,  0.35f }; // 피해 계수
    // private float[] acceleration     = new float[MAX_LEVEL] { 1.00f,  1.00f,  2.00f,  2.00f,  3.00f }; // 가속도
    private int[] spiritCount        = new int[MAX_LEVEL]   {     3,      5,      5,      7,      7 }; // 정령 수

    protected override float AttackInterval => 0.5f;
    public float ChargeTime                 => chargeTime[level-1];
    public float RunningTime                => runningTime[level-1];
    public float Damage                     => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    public float Acceleration               => 3f;
    public int Count                        => spiritCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Spirit",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Summon spirits that chase and hit monsters. Spirits has to return and charge after flies certain time."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nRunning Time : <color=#f40>{runningTime[0]}sec</color>"
                   + $"\nCharge Time : <color=#f40>{chargeTime[0]}sec</color>"
                   + $"\nNumber of Spirits : <color=#f40>{spiritCount[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Summon spirits that chase and hit monsters. Spirits has to return and charge after flies certain time."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nRunning Time : <color=#f40>{runningTime[level-1]}sec</color>"
                   + $"\nCharge Time : <color=#f40>{chargeTime[level-1]}sec</color> > <color=#f40>{chargeTime[NextLevelIndex]}sec</color>"
                   + $"\nNumber of Spirits : <color=#f40>{spiritCount[level-1]}</color> > <color=#f40>{spiritCount[NextLevelIndex]}</color>"
                   + $"</nobr>"
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "정령",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"몬스터를 추적하고 공격하는 정령을 소환합니다. 정령은 일정 시간 비행 후에 돌아와 충전 해야합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n비행 시간 : <color=#f40>{runningTime[0]}초</color>"
                   + $"\n충전 시간 : <color=#f40>{chargeTime[0]}초</color>"
                   + $"\n정령 수 : <color=#f40>{spiritCount[0]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"몬스터를 추적하고 공격하는 정령을 소환합니다. 정령은 일정 시간 비행 후에 돌아와 충전 해야합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n비행 시간 : <color=#f40>{runningTime[level-1]}초</color>"
                   + $"\n충전 시간 : <color=#f40>{chargeTime[level-1]}초</color> > <color=#f40>{chargeTime[NextLevelIndex]}초</color>"
                   + $"\n정령 수 : <color=#f40>{spiritCount[level-1]}</color> > <color=#f40>{spiritCount[NextLevelIndex]}</color>"
                   + $"</nobr>"
            }
    );
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