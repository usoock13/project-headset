using System.Collections;
using UnityEngine;

public class AWeaponMultiUseAxe : Weapon {
    [SerializeField] private EffectAwAxe projectile;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] staticDamage = new float[MAX_LEVEL] {   80f,     95f,    110f,    125f,    140f, }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]   { 0.40f,   0.40f,   0.40f,   0.40f,   0.40f, }; // 피해 계수

    protected override float AttackInterval => 2.5f;
    public float MaxFlyingTime => 15f;
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "The Piece Maker",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Throw a spinning axe to damage monsters. Axe flys around character until the character catch it."
                   + $"\n"
                   + $"\nDPS : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nMax Flying Time : <color=#f40>{MaxFlyingTime}sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Throw a spinning axe to damage monsters. Axe flys around character until the character catch it."
                   + $"\n"
                   + $"\nDPS : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nMax Flying Time : <color=#f40>{MaxFlyingTime}sec</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "피스메이커",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"회전하는 도끼를 던져 몬스터에게 피해를 가합니다. 도끼는 캐릭터가 잡기 전까지 캐릭터 주변을 날아다닙니다."
                   + $"\n"
                   + $"\n초당 피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n최대 비행 시간 : <color=#f40>{MaxFlyingTime}초</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"회전하는 도끼를 던져 몬스터에게 피해를 가합니다. 도끼는 캐릭터가 잡기 전까지 캐릭터 주변을 날아다닙니다."
                   + $"\n"
                   + $"\n초당 피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n최대 비행 시간 : <color=#f40>{MaxFlyingTime}초</color>"
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
        EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
}