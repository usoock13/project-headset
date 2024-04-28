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

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "The Canaan",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Spread the Promised Land that continuous damage monsters in the area. During Character not walking the area extends. Each time monsters defeated in area a few times, the believer that asists the character is summoned."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttakc Interval : <color=#f40>{AttackInterval}sec</color>"
                   + $"\nBeliever Damage : <color=#f40>{allyStaticDamage[0]}+{allyDamageCoef[0]*100}%</color>"
                   + $"\nBeliever Duration : <color=#f40>{AllyDuration}sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Spread the Promised Land that continuous damage monsters in the area. During Character not walking the area extends. Each time monsters defeated in area a few times, the believer that asists the character is summoned."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttakc Interval : <color=#f40>{AttackInterval}sec</color>"
                   + $"\nBeliever Damage : <color=#f40>{allyStaticDamage[level-1]}+{allyDamageCoef[level-1]*100}%</color> > <color=#f40>{allyStaticDamage[NextLevelIndex]}+{allyDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nBeliever Duration : <color=#f40>{AllyDuration}sec</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "가나안",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"약속의 땅을 펼쳐 범위 안의 몬스터에게 피해를 가합니다. 영역은 캐릭터가 걷지 않으면 점점 넓어집니다. 몬스터가 영역 내에게 일정 횟수 처치 될 때 마다 캐릭터를 돕는 신자가 소환합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"\n신자 피해량 : <color=#f40>{allyStaticDamage[0]}+{allyDamageCoef[0]*100}%</color>"
                   + $"\n신자 지속 시간 : <color=#f40>{AllyDuration}초</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"약속의 땅을 펼쳐 범위 안의 몬스터에게 피해를 가합니다. 영역은 캐릭터가 걷지 않으면 점점 넓어집니다. 몬스터가 영역 내에게 일정 횟수 처치 될 때 마다 캐릭터를 돕는 신자가 소환합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"\n신자 피해량 : <color=#f40>{allyStaticDamage[level-1]}+{allyDamageCoef[level-1]*100}%</color> > <color=#f40>{allyStaticDamage[NextLevelIndex]}+{allyDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n신자 지속 시간 : <color=#f40>{AllyDuration}초</color>"
                   + $"</nobr>",
            }
    );
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

    public override bool Filter() => GameManager.instance.StageManager.Character.level >= 20;
}