using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AWeaponExplosionArrow : Weapon {
    [SerializeField] private EffectAwNormalArrow normalArrowOrigin;
    [SerializeField] private EffectAwExplosionArrow explosionArrowOrigin;
    public ObjectPooler NormalEffectPooler { get; private set; }
    public ObjectPooler ExplosionEffectPooler { get; private set; }

    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] damageCoef = new float[MAX_LEVEL]       {  0.15f,  0.15f,  0.15f,  0.15f,  0.15f };  // 피해 계수
    private float[] staticDamage = new float[MAX_LEVEL]     {    20f,    25f,    30f,    35f,    40f };  // 고정 피해량
    private int[] arrowBombCount = new int[MAX_LEVEL]       {     11,     10,      9,      8,      7 };  // 폭발 화살 발사를 위한 적중 횟수
    private float[] bombDamageCoef = new float[MAX_LEVEL]   {  1.55f,  1.55f,  1.55f,  1.55f,  1.55f };  // 폭발 화살 피혜 계수
    private float[] bombStaticDamage = new float[MAX_LEVEL] {    70f,    90f,   110f,   130f,   150f };  // 폭발 화살 고정 피해량
    protected override float AttackInterval => 0.12f;
    public float HittingDelay => 0.25f;
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float BombDamage => bombDamageCoef[level-1] * _Character.Power + bombStaticDamage[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "The Locksmith",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Shoot several arrows. Each time arrows hit monster a few times, character shoot a explosion arrow."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{AttackInterval}sec</color>"
                   + $"\nHit for Explosion Arrow : <color=#f40>{arrowBombCount[0]}</color>"
                   + $"\nExplosion Damage : <color=#f40>{bombStaticDamage[0]}+{bombDamageCoef[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Shoot several arrows. Each time arrows hit monster a few times, character shoot a explosion arrow."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{AttackInterval}sec</color>"
                   + $"\nHit for Explosion Arrow : <color=#f40>{arrowBombCount[level-1]}</color> > <color=#f40>{arrowBombCount[NextLevelIndex]}</color>"
                   + $"\nExplosion Damage : <color=#f40>{bombStaticDamage[level-1]}+{bombDamageCoef[level-1]*100}%</color> > <color=#f40>{bombStaticDamage[NextLevelIndex]}+{bombDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "열쇠공",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"여러개의 화살을 발사합니다. 화살이 일정 횟수 몬스터에게 적중할 때 마다 폭발하는 화살을 추가로 발사합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"\n폭발 화살을 위한 적중 수 : <color=#f40>{arrowBombCount[0]}</color>"
                   + $"\n폭발 피해량 : <color=#f40>{bombStaticDamage[0]}+{bombDamageCoef[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"여러개의 화살을 발사합니다. 화살이 일정 횟수 몬스터에게 적중할 때 마다 폭발하는 화살을 추가로 발사합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"\n폭발 화살을 위한 적중 수 : <color=#f40>{arrowBombCount[level-1]}</color> > <color=#f40>{arrowBombCount[NextLevelIndex]}</color>"
                   + $"\n폭발 피해량 : <color=#f40>{bombStaticDamage[level-1]}+{bombDamageCoef[level-1]*100}%</color> > <color=#f40>{bombStaticDamage[NextLevelIndex]}+{bombDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information
    
    private int hitCount = 0;

    private void Awake() {
        NormalEffectPooler = new ObjectPooler(
            poolingObject: normalArrowOrigin.gameObject,
            parent: this.transform,
            count: 100, 
            restoreCount: 50
        );
        ExplosionEffectPooler = new ObjectPooler(
            poolingObject: explosionArrowOrigin.gameObject,
            parent: this.transform,
            count: 10,
            restoreCount: 5
        );
    }
    protected override void Attack() {
        GameObject arrowInstance = NormalEffectPooler.OutPool(transform.position + Vector3.up * 0.5f, _Character.attackArrow.rotation);
        float aimJitter = UnityEngine.Random.Range(-7f, 7f);
        arrowInstance.transform.Rotate(Vector3.forward, aimJitter);
        _Character.OnAttack();
    }

    public void OnAttackMonster() {
        hitCount ++;
        if(hitCount >= arrowBombCount[level-1]) {
            hitCount -= arrowBombCount[level-1];
            
            ExplosionEffectPooler.OutPool(transform.position + Vector3.up * 0.5f, _Character.attackArrow.rotation);
            _Character.OnAttack();
        }
    }
    
    public override bool Filter() => GameManager.instance.StageManager.Character.level >= 20;
}