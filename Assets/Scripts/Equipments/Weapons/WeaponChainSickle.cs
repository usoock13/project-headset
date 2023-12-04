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
    public override Sprite Icon => _weaponIcon;
    public override string Name => "제니의 사슬낫";
    public override string Description =>
        (level+1) switch {
            5 => $"<color=#f40>{interval[level]}</color>초마다 조준 방향으로 자비 없는 사슬낫을 던져 적중하는 모든 적에게 <color=#f40>{staticDamage[level]}+{damageCoef[level]*100}%</color>의 피해를 가합니다.\n"
               + $"낫은 최대 사거리에 도달하면 그 자리에 꽂힙니다. 낫은 최대 3개까지 꽂혀 있을 수 있으며, 최대 개수를 초과하면 가장 먼저 던진 낫을 회수하여 <color=#f40>{pullingStaticDamage[level]}+{pullingDamageCoef[level]*100}%</color>의 피해를 입힙니다.\n"
               + $"회피를 사용하여 꽂혀있는 모든 낫을 회수할 수 있습니다.",
            _ => $"<color=#f40>{interval[level]}</color>초마다 조준 방향으로 자비 없는 사슬낫을 던져 적중하는 모든 적에게 <color=#f40>{staticDamage[level]}+{damageCoef[level]*100}%</color>의 피해를 가합니다.\n"
               + $"낫은 최대 사거리에 도달하면 그 자리에 꽂힙니다. 낫은 최대 3개까지 꽂혀 있을 수 있으며, 최대 개수를 초과하면 가장 먼저 던진 낫을 회수하여 <color=#f40>{pullingStaticDamage[level]}+{pullingDamageCoef[level]*100}%</color>의 피해를 입힙니다.\n"
        };
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
    private void RecallAll() {
        while(hookeds.Count > 0)
            hookeds.Dequeue().PullSickle();
    }
    private void OnDodge(Character character) {
        if(level >= 5)
            RecallAll();
    }
}