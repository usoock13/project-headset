using System.Collections;
using UnityEngine;

public class WeaponAxe : Weapon {
    [SerializeField] private EffectFlyingAxe projectile;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] interval = new float[MAX_LEVEL]         { 2.5f,    2.5f,    2.5f,    2.5f,    2.5f, }; // 공격 간격
    private float[] staticDamage = new float[MAX_LEVEL]     {  20f,     30f,     40f,     50f,     60f, }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]       { 0.5f,    0.6f,    0.7f,    0.8f,    0.9f, }; // 피해 계수
    private float[] projectileScale = new float[MAX_LEVEL]  { 1.0f,    1.0f,    1.5f,    1.5f,    2.0f, }; // 투사체 크기
    private int[] maxHitCount = new int[MAX_LEVEL]          {    4,       7,      10,      13,      99, }; // 최대 관통 횟수

    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float ProjectileScale => projectileScale[level-1];
    public int MaxHitCount => maxHitCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    
    [SerializeField] private Sprite _weaponIcon;
    [SerializeField] private ItemAwake itemAwake;
    
    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Axe",
        Description: 
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"Throw a axe to damage the monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"\nMax Hit Monsters : <color=#f40>{maxHitCount[0]}</color>"
                   + $"\nAxe Scale : <color=#f40>{projectileScale[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Throw a axe to damage the monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\nMax Hit Monsters : <color=#f40>{maxHitCount[level-1]}</color> > <color=#f40>{maxHitCount[NextLevelIndex]}</color>"
                   + $"\nAxe Scale : <color=#f40>{projectileScale[level-1]*100}%</color> > <color=#f40>{projectileScale[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "도끼",
        Description: 
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"관통한 몬스터에게 피해를 가하는 도끼를 던집니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"\n최대 공격 횟수 : <color=#f40>{maxHitCount[0]}</color>"
                   + $"\n도끼 크기 : <color=#f40>{projectileScale[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"관통한 몬스터에게 피해를 가하는 도끼를 던집니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"\n최대 공격 횟수 : <color=#f40>{maxHitCount[level-1]}</color> > <color=#f40>{maxHitCount[NextLevelIndex]}</color>"
                   + $"\n도끼 크기 : <color=#f40>{projectileScale[level-1]*100}%</color> > <color=#f40>{projectileScale[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information

    private void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: projectile.gameObject,
            parent: this.transform);
    }
    protected override void Attack() {
        EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }

    protected override void OnLevelUp() {
        base.OnLevelUp();
        if(level == MaxLevel) {
            foreach(Character character in GameManager.instance.StageManager.Party) {
                if(character is CharacterMercenary) {
                    GameManager.instance.StageManager.EquipmentsManager.AddBonusItemAtList(itemAwake);
                    break;
                }
            }
        }
    }
}