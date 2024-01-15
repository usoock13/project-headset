using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChainSickle : Weapon {
    [SerializeField] private EffectChainSickle projectile;
    public ObjectPooler EffectPooler { get; private set; }
    public ObjectPooler AttachmentPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] interval = new float[MAX_LEVEL]                { 1.2f,    1.2f,    1.2f,    0.8f,    0.8f, }; // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]              { 0.1f,    0.1f,    0.1f,    0.1f,    0.1f, }; // 던지기 피해계수
    private float[] staticDamage = new float[MAX_LEVEL]            {  10f,     15f,     20f,     25f,     30f, }; // 던지기 고정 피해량
    private float[] pullingDamageCoef = new float[MAX_LEVEL]       { 0.8f,    0.8f,    0.8f,    0.8f,    0.8f, }; // 당기기 피해계수
    private float[] pullingStaticDamage = new float[MAX_LEVEL]     {  06f,     07f,     08f,     09f,     10f, }; // 당기기 고정 피해량
    private int[] maxHookedsCount = new int[MAX_LEVEL]             {    3,       3,       3,       5,       5, }; // 최대 낫 개수

    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float PullingDamage => pullingDamageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public int MaxHookedsCount => maxHookedsCount[level-1];
    protected Queue<EffectChainSickle> hookeds = new Queue<EffectChainSickle>();
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    
    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Chain Sickle",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Throw a sickle to damage monsters. The sickle stays there when it reaches maximum range. When the number of sickles thrown exceeds maximum, the oldest sickle returns to character and damages monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nReturning Damage : <color=#f40>{pullingStaticDamage[0]}+{pullingDamageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"\nMax Sickle Count : <color=#f40>{maxHookedsCount[0]}</color>"
                   + $"</nobr>",
                4 => $"<nobr>"
                   + $"Throw a sickle to damage monsters. The sickle stays there when it reaches maximum range. When the number of sickles thrown exceeds maximum, the oldest sickle returns to character and damages monsters."
                   + $"\n<color=#f40>Now when character does dodge, all sickles return!</color>"
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nReturning Damage : <color=#f40>{pullingStaticDamage[level-1]}+{pullingDamageCoef[level-1]*100}%</color> > <color=#f40>{pullingStaticDamage[NextLevelIndex]}+{pullingDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\nMax Sickle Count : <color=#f40>{maxHookedsCount[level-1]}</color> > <color=#f40>{maxHookedsCount[NextLevelIndex]}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Throw a sickle to damage monsters. The sickle stays there when it reaches maximum range. When the number of sickles thrown exceeds maximum, the oldest sickle returns to character and damages hit monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nReturning Damage : <color=#f40>{pullingStaticDamage[level-1]}+{pullingDamageCoef[level-1]*100}%</color> > <color=#f40>{pullingStaticDamage[NextLevelIndex]}+{pullingDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\nMax Sickle Count : <color=#f40>{maxHookedsCount[level-1]}</color> > <color=#f40>{maxHookedsCount[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "사슬낫",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"적중한 몬스터에게 피해를 주는 낫을 던집니다. 낫은 최대 사거리에 도달하면 그 자리에 머무릅니다. 던진 낫이 최대 개수를 초과하면 가장 이전에 던진 낫이 되돌아오며 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n복귀 피해량 : <color=#f40>{pullingStaticDamage[0]}+{pullingDamageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"\n최개 개수 : <color=#f40>{maxHookedsCount[0]}개</color>"
                   + $"</nobr>",
                4 => $"<nobr>"
                   + $"적중한 몬스터에게 피해를 주는 낫을 던집니다. 낫은 최대 사거리에 도달하면 그 자리에 머무릅니다. 던진 낫이 최대 개수를 초과하면 가장 이전에 던진 낫이 되돌아오며 피해를 가합니다."
                   + $"\n<color=#f40>이제 캐릭터가 회피를 사용하면 모든 낫이 되돌아옵니다!</color>"
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n복귀 피해량 : <color=#f40>{pullingStaticDamage[level-1]}+{pullingDamageCoef[level-1]*100}%</color> > <color=#f40>{pullingStaticDamage[NextLevelIndex]}+{pullingDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"\n최개 개수 : <color=#f40>{maxHookedsCount[level-1]}개</color> > <color=#f40>{maxHookedsCount[NextLevelIndex]}개</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"적중한 몬스터에게 피해를 주는 낫을 던집니다. 낫은 최대 사거리에 도달하면 그 자리에 머무릅니다. 던진 낫이 최대 개수를 초과하면 가장 이전에 던진 낫이 되돌아오며 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n복귀 피해량 : <color=#f40>{pullingStaticDamage[level-1]}+{pullingDamageCoef[level-1]*100}%</color> > <color=#f40>{pullingStaticDamage[NextLevelIndex]}+{pullingDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"\n최개 개수 : <color=#f40>{maxHookedsCount[level-1]}개</color> > <color=#f40>{maxHookedsCount[NextLevelIndex]}개</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information

    private void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: projectile.gameObject,
            parent: this.transform
        );
    }
    protected override void Attack() {
        var effect = EffectPooler.OutPool(transform.position, _Character.attackArrow.rotation).GetComponent<EffectChainSickle>();
        _Character.OnAttack();
        if(effect != null)
            hookeds.Enqueue(effect);
        if(hookeds.Count > MaxHookedsCount) {
            hookeds.Dequeue().PullSickle();
        }
    }
    public override void OnGotten() {
        base.OnGotten();
        _Character.onDodge += OnDodge;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.onDodge -= OnDodge;
        RecallAll();
    }
    private void RecallAll() {
        while(hookeds.Count > 0)
            hookeds.Dequeue().PullSickle();
    }
    private void OnDodge(Character character) {
        if(level >= 5)
            RecallAll();
    }
}