using System;
using System.Collections;
using UnityEngine;

public class WeaponGreatSword : Weapon {
    [SerializeField] private EffectGreatSword swordEffect;
    private ObjectPooler effectPooler;
    [SerializeField] private float attackRange = .5f;

    [SerializeField] ItemAwake itemAwake;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]      {   2f,     2f,   1.5f,    1.5f,   1.5f }; // 공격 간격
    private float[] staticDamage = new float[MAX_LEVEL]  {  30f,    45f,    45f,     70f,   155f }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]    { 0.6f,   0.9f,   0.9f,    1.0f,   2.5f }; // 피해 계수
    private float[] areaScale = new float[MAX_LEVEL]     {   1f,     1f,     1f,   1.25f,   2.0f }; // 공격 범위 축척
    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float AreaScale => areaScale[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Short Sword",
        Description:
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"Wield a sword to damage the monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"\nAttack Area : <color=#f40>{areaScale[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Wield a sword to damage the monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\nAttack Area : <color=#f40>{areaScale[level-1]*100}%</color> > <color=#f40>{areaScale[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "숏소드",
        Description:
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"검을 휘둘러 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"\n공격 범위 : <color=#f40>{areaScale[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"검을 휘둘러 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color> > <color=#f40>{interval[NextLevelIndex]}초</color>"
                   + $"\n공격 범위 : <color=#f40>{areaScale[level-1]*100}%</color> > <color=#f40>{areaScale[NextLevelIndex]*100}%</color>"
                   + $"</nobr>"
            }
    );
    #endregion Weapon Information

    private void Awake() {
        effectPooler = new ObjectPooler(poolingObject: swordEffect.gameObject, parent: this.transform);
    }
    protected override void Attack() {
        Vector3 effectPoint = _Character.attackArrow.position + _Character.attackArrow.forward*attackRange;
        GameObject instance = effectPooler.OutPool(effectPoint, _Character.attackArrow.rotation);
        StartCoroutine(InPoolEffect(5f, instance));
        _Character.OnAttack();
    }
    private IEnumerator InPoolEffect(float delay, GameObject effect) {
        yield return new WaitForSeconds(delay);
        effectPooler.InPool(effect);
    }

    protected override void OnLevelUp() {
        base.OnLevelUp();
        if(level == MaxLevel) {
            foreach(Character character in GameManager.instance.StageManager.Party) {
                if(character is CharacterWarrior) {
                    GameManager.instance.StageManager.EquipmentsManager.AddBonusItemAtList(itemAwake);
                    break;
                }
            }
        }
    }
}