using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponShortbow : Weapon {
    [SerializeField] private EffectShortbow shortbowEffect;
    private ObjectPooler effectPooler;
    
    [SerializeField] private ItemAwake itemAwake;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]     {   .5f,     .4f,     .3f,      .3f,     .3f };  // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]   {  .10f,    .10f,    .10f,     .15f,    .15f };  // 피해 계수
    private float[] staticDamage = new float[MAX_LEVEL] {    12f,    16f,     16f,      24f,     24f };  // 고정 피해량
    private int[] arrowQuantity = new int[MAX_LEVEL]    {     1,       1,       2,        2,       4 };  // 투사체 수
    protected override float AttackInterval => interval[level-1];
    public float HittingDelay => 0.25f;
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float ArrowQuantity => arrowQuantity[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Shortbow",
        Description:
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"Shoot arrows to damage the monster."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"\nArrows Quantity : <color=#f40>{arrowQuantity[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Shoot arrows to damage the monster."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\nArrows Quantity : <color=#f40>{arrowQuantity[level-1]}</color> > <color=#f40>{arrowQuantity[NextLevelIndex]}</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "단궁",
        Description:
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"화살을 발사해 처음 적중한 몬스터에게 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"\n화살 개수 : <color=#f40>{arrowQuantity[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"화살을 발사해 처음 적중한 몬스터에게 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"\n화살 개수 : <color=#f40>{arrowQuantity[level-1]}</color> > <color=#f40>{arrowQuantity[NextLevelIndex]}</color>"
                   + $"</nobr>"
            }
    );

    private void Awake() {
        effectPooler = new ObjectPooler(
            poolingObject: shortbowEffect.gameObject,
            onCreated: (gobj) => {
                var effect = gobj.GetComponent<EffectShortbow>();
                effect.originWeapon = this;
                effect.onDisappear += (projectile) => {
                    effectPooler.InPool(projectile.gameObject);
                };
            },
            parent: this.transform,
            count: 100, 
            restoreCount: 50
        );
    }
    protected override void Attack() {
        for(int i=0; i<ArrowQuantity; i++) {
            GameObject arrowInstance = effectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
            float aimJitter = UnityEngine.Random.Range(-7f, 7f);
            arrowInstance.transform.Rotate(Vector3.forward, aimJitter);
            _Character.OnAttack();
        }
    }
    #endregion Weapon Information

    protected override void OnLevelUp() {
        base.OnLevelUp();
        if(level == MaxLevel) {
            foreach(Character character in GameManager.instance.StageManager.Party) {
                if(character is CharacterRanger) {
                    GameManager.instance.StageManager.EquipmentsManager.AddBonusItemAtList(itemAwake);
                    break;
                }
            }
        }
    }
}