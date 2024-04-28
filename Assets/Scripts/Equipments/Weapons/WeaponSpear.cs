using System.Collections;
using UnityEngine;

public class WeaponSpear : Weapon {
    [SerializeField] private EffectSpear effectOrigin;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]         {   1.4f,    1.2f,    1.0f,    1.0f,    1.0f,  }; // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]       {   0.5f,    0.5f,    0.5f,    0.5f,    0.5f,  }; // 피해 계수
    private float[] staticDamage = new float[MAX_LEVEL]     {    15f,     15f,     30f,     30f,     50f,  }; // 고정 피해량
    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    #endregion Weapon Status

    #region Weapon Information

    [SerializeField] private Sprite _weaponIcon;
    [SerializeField] private ItemAwake itemAwake;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Long Spear",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Pierce with a spear to damage monsters hit."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Pierce with a spear to damage monsters hit."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "장창",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"창으로 찔러 적중한 몬스터에게 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"창으로 찔러 적중한 몬스터에게 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information

    private void Awake() {
        EffectPooler = new ObjectPooler(poolingObject: effectOrigin.gameObject, parent: this.transform);
    }

    protected override void Attack() {
        GameObject mainEffect = EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        StartCoroutine(InPoolEffect(5f, mainEffect, EffectPooler));
        _Character.OnAttack();
    }

    private IEnumerator InPoolEffect(float delay, GameObject effect, ObjectPooler pooler) {
        yield return new WaitForSeconds(delay);
        pooler.InPool(effect);
    }

    protected override void OnLevelUp() {
        base.OnLevelUp();
        if(level == MaxLevel) {
            foreach(Character character in GameManager.instance.StageManager.Party) {
                if(character is CharacterAdventurer) {
                    GameManager.instance.StageManager.EquipmentsManager.AddBonusItemAtList(itemAwake);
                    break;
                }
            }
        }
    }
}