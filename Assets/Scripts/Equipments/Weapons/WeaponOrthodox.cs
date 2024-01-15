using System;
using System.Collections;
using UnityEngine;

public class WeaponOrthodox : Weapon {
    [SerializeField] private EffectStraightPunch punchEffect;
    public ObjectPooler EffectPooler { get; private set; }
    [SerializeField] float attackRange = .5f;

    [SerializeField] private ItemAwake itemAwake;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]        { 1.3f,    1.3f,    1.3f,    1.3f,    1.3f }; // 공격 간격
    private int[] attackCount = new int[MAX_LEVEL]         {    2,       2,       3,       3,       5 }; // 공격 횟수
    private float[] staticDamage = new float[MAX_LEVEL]    {  10f,     20f,     20f,     30f,     30f }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]      { 0.3f,    0.3f,    0.3f,    0.3f,    0.3f }; // 피해 계수
    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Knuckle",
        Description:
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"Throw several short punchs to damage monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nPunch Count : <color=#f40>{attackCount[0]}</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Throw several short punchs to damage monsters."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nPunch Count : <color=#f40>{attackCount[level-1]}</color> > <color=#f40>{attackCount[NextLevelIndex]}</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "너클",
        Description:
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"짧은 거리에 주먹을 여러번 날려 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n펀치 횟수 : <color=#f40>{attackCount[0]}</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}초</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"짧은 거리에 주먹을 여러번 날려 피해를 가합니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n펀치 횟수 : <color=#f40>{attackCount[level-1]}</color> > <color=#f40>{attackCount[NextLevelIndex]}</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}초</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information

    private void Awake() {
        EffectPooler = new ObjectPooler(poolingObject: punchEffect.gameObject, parent: this.transform);
    }
    protected override void Attack() {
        StartCoroutine(AttackCoroutine());
        _Character.OnAttack();
    }
    private IEnumerator AttackCoroutine() {
        for(int i=0; i<attackCount[level-1]; i++) {
            var instance = EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
            instance.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(-30f, 30f));
            StartCoroutine(InPoolEffect(3f, instance, EffectPooler));
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator InPoolEffect(float delay, GameObject effect, ObjectPooler pooler) {
        yield return new WaitForSeconds(delay);
        pooler.InPool(effect);
    }

    protected override void OnLevelUp() {
        base.OnLevelUp();
        if(level == MaxLevel) {
            foreach(Character character in GameManager.instance.StageManager.Party) {
                if(character is CharacterFighter) {
                    GameManager.instance.StageManager.EquipmentsManager.AddBonusItemAtList(itemAwake);
                    break;
                }
            }
        }
    }
}