using System.Collections;
using UnityEngine;

public class AweaponHolyLand : Weapon {
    [SerializeField] private EffectAwHolyLand effects;

    [SerializeField] private EffectHallyKnight knightOrigin;
    public ObjectPooler KnightPooler { get; private set; }

    [SerializeField] private AttachmentRepentance repentanceOrigin;
    public ObjectPooler RepentancePooler;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] damageCoef = new float[MAX_LEVEL]        {  0.2f,    0.2f,    0.2f,    0.2f,    0.2f, }; // 고정 피해량
    private float[] staticDamage = new float[MAX_LEVEL]      {   50f,     60f,     70f,     80f,     90f, }; // 고정 피해량
    private int[]   killCountToSummon = new int[MAX_LEVEL]   {    13,      11,      10,       8,       7, }; // 소환에 필요한 킬 카운트

    private float[] allyDamageCoef = new float[MAX_LEVEL]    { 0.25f,   0.25f,   0.25f,  0.25f,   0.25f, };
    private float[] allyStaticDamage = new float[MAX_LEVEL]  {  100f,    125f,    150f,   175f,    200f, };

    protected override float AttackInterval => 0.5f;
    public float DamageInterval => AttackInterval;
    public float DamagePerSecond => staticDamage[level-1];
    public float HittingDelay => 0.25f;

    public float AllyDamage => allyDamageCoef[level-1] * _Character.Power + allyStaticDamage[level-1];
    public float AllyDuration => 10f;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "거룩한 땅";
    public override string Description =>
        $"<nobr>"
      + $"거룩한 땅을 펼쳐 매초 범위 내의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.\n"
      + $"영역 안에서 적이 <color=#f40>{killCountToSummon[NextLevelIndex]}</color>번 처치될 때마다, 적을 추격하여 매 초 <color=#f40>{allyStaticDamage[NextLevelIndex]}+{allyDamageCoef[NextLevelIndex]}%</color>의 피해를 가하는 빛의 기사를 소환합니다."
      + $"빛의 기사는 {AllyDuration}초간 지속됩니다."
      + $"이 무기는 공격 속도에 영향을 받지 않습니다."
      + $"</nobr>";
    #endregion Weapon Information

    private int killCount = 0;

    private void Awake() {
        KnightPooler = new ObjectPooler(
            poolingObject: knightOrigin.gameObject,
            parent: this.transform
        );

        RepentancePooler = new ObjectPooler(
            poolingObject: repentanceOrigin.gameObject,
            parent: this.transform,
            count: 50
        );
    }
    protected override void Update() {}

    protected override void Attack() {}
    public override void OnEquipped() {
        base.OnEquipped();
        effects.gameObject.SetActive(true);
    }

    protected override void OnLevelUp() {
        base.OnLevelUp();
        if(killCount > killCountToSummon[level-1]) {
            SummonAlly();
            killCount = 0;
        }
        UpdateKillCountUI();
    }

    public void OnKillMonster() {
        killCount ++;
        if(killCount >= killCountToSummon[level-1]) {
            SummonAlly();
            killCount = 0;
        }
        UpdateKillCountUI();
    }

    private void SummonAlly() {
        var knight = KnightPooler.OutPool().GetComponent<EffectHallyKnight>();
        if(knight != null) {
            knight.Active(AllyDamage, AllyDuration);
        }
    }

    private void UpdateKillCountUI() {
        extraInformation = (killCountToSummon[level-1] - killCount).ToString();
        GameManager.instance.StageManager.StageUIManager.UpdateWeaponList();
    }
}