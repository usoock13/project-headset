using System.Collections;
using UnityEngine;

public class AWeaponElementalTrio : Weapon {
    [SerializeField] private EffectAwElement[] effects;

    [SerializeField] private AttachmentBurning attchBurning;
    [SerializeField] private AttachmentFreeze attchFreeze;
    [SerializeField] private AttachmentShock attchShock;

    public ObjectPooler BurningPooler { get; private set; }
    public ObjectPooler FreezePooler { get; private set; }
    public ObjectPooler ShockPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] staticDamage    = new float[MAX_LEVEL] {   50f,     60f,    70f,     80f,     90f }; // 고정 피해량
    private float[] damageCoef      = new float[MAX_LEVEL] { 0.55f,   0.55f,  0.55f,   0.55f,   0.55f }; // 피해 계수

    private float[] burningStaticDamage = new float[MAX_LEVEL] {   30f,    40f,    50f,    60f,    70f, }; // 점화 고정 피해량
    private float[] burningDamageCoef   = new float[MAX_LEVEL] { 0.25f,  0.25f,  0.25f,  0.25f,  0.25f, }; // 점화 피해 계수

    private float[] freezeDuration = new float[MAX_LEVEL] { 0.8f,  0.9f,  1.0f,  1.1f,  1.2f, }; // 빙결 지속 시간

    private float[] shockStaticDamage = new float[MAX_LEVEL] {   10f,    15f,    20f,    25f,    30f, }; // 감전 고정 피해량
    private float[] shockDamageCoef   = new float[MAX_LEVEL] { 0.10f,  0.10f,  0.10f,  0.10f,  0.10f, }; // 감전 피해 계수

    protected override float AttackInterval => 999;
    public float ReactiveInterval => 1.5f;
    public float Damage => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;

    public float BurningDamage   => burningStaticDamage[level-1] + burningDamageCoef[level-1] * _Character.Power; // 점화 피해량
    public float BurningDuration => 4f;
    public float FreezeDuration  => freezeDuration[level-1]; // 빙결 피해량
    public float ShockDamage     => shockStaticDamage[level-1] + shockDamageCoef[level-1] * _Character.Power; // 감전 피해량
    public float ShockDuration   => 4f;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "원소 트리오";
    public override string Description =>
                 $"<nobr>"
               + $"주변을 공전하는 불과 얼음, 번개 구체를 각각 2개씩 소환합니다. 구체는 적과 충돌하면 폭발하여 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고"
               + $"속성에따라 추가 효과를 적용합니다. 폭발한 구체는 {ReactiveInterval}초 후 재생성 됩니다."
               + $"<i>불 : 매 초 <color=#f40>{burningStaticDamage[NextLevelIndex]}+{burningDamageCoef[NextLevelIndex]}%</color>의 피해를 가하는 점화 효과를 {BurningDuration}초간 적용합니다.</i>"
               + $"<i>얼음 : <color=#f40>{freezeDuration[NextLevelIndex]}</color>초동안 적은 빙결시킵니다.</i>"
               + $"<i>번개 : 최대 3명의 주변 적에게 매 초 <color=#f40>{shockStaticDamage[NextLevelIndex]}+{shockDamageCoef[NextLevelIndex]}%</color>의 피해를 가하는 감전 효과를 {ShockDuration}초간 적용합니다.</i>"
               + $"</nobr>";
    #endregion Weapon Information
    protected override void Update() {}
    protected override void Attack() {}

    private void Awake() {
        BurningPooler = new ObjectPooler(
            poolingObject: attchBurning.gameObject,
            parent: this.transform
        );
        FreezePooler = new ObjectPooler(
            poolingObject: attchFreeze.gameObject,
            parent: this.transform
        );
        ShockPooler = new ObjectPooler(
            poolingObject: attchShock.gameObject,
            parent: this.transform
        );
    }

    public override void OnEquipped() {
        base.OnEquipped();
        for(int i=0; i<effects.Length; i++) {
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
}