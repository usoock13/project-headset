using System.Collections;
using UnityEngine;

public class WeaponFireRing : Weapon {
    [SerializeField] private EffectFireRing[] effects;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval        = new float[MAX_LEVEL] { 5.00f,   5.00f,  3.00f,   3.00f,   1.50f }; // 재생성 간격
    private float[] staticDamage    = new float[MAX_LEVEL] {   10f,     15f,    15f,     25f,     35f }; // 고정 피해량
    private float[] damageCoef      = new float[MAX_LEVEL] { 0.40f,   0.40f,  0.40f,   0.40f,   0.40f }; // 피해 계수
    protected override float AttackInterval => 999;
    public float ReactiveInterval => interval[level-1];
    public float Damage => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Fireballs",
        Description:
            NextLevelIndex switch {
                0 => "<nobr>"
                  + $"Summon a spinning fireball around the character. Fireball hits a monster and then explodes, damages to the monsters in around. It's need some time to regenerate exploded fireball."
                  + $"\n"
                  + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                  + $"\nTime of Regenerate : <color=#f40>{interval[0]}</color>"
                  + $"\nNumber of Fireballs : <color=#f40>3</color>"
                  + $"</nobr>",
                _ => "<nobr>"
                  + $"Summon a spinning fireball around the character. Fireball hits a monster and then explodes, damages to the monsters in around. It's need some time to regenerate exploded fireball."
                  + $"\n"
                  + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                  + $"\nTime of Regenerate : <color=#f40>{interval[level-1]}</color> > <color=#f40>{interval[NextLevelIndex]}</color>"
                  + $"\nNumber of Fireballs : <color=#f40>3</color> > <color=#f40>{(NextLevelIndex == MaxLevel-1 ? 6 : 3)}</color>"
                  + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "화염구",
        Description:
            NextLevelIndex switch {
                0 => "<nobr>"
                  + $"캐릭터 주변을 회전하는 화염구를 소환합니다. 화염구는 몬스터와 충돌하면 폭발하여 주변에 피해를 가하고 잠시 뒤 재생성됩니다."
                  + $"\n"
                  + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                  + $"\n재생성 시간 : <color=#f40>{interval[0]}</color>"
                  + $"\n화염구 개수 : <color=#f40>3</color>"
                  + $"</nobr>",
                _ => "<nobr>"
                  + $"캐릭터 주변을 회전하는 화염구를 소환합니다. 화염구는 몬스터와 충돌하면 폭발하여 주변에 피해를 가하고 잠시 뒤 재생성됩니다."
                  + $"\n"
                  + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                  + $"\n재생성 시간 : <color=#f40>{interval[level-1]}</color> > <color=#f40>{interval[NextLevelIndex]}</color>"
                  + $"\n화염구 개수 : <color=#f40>3</color> > <color=#f40>{(NextLevelIndex == MaxLevel-1 ? 6 : 3)}</color>"
                  + $"</nobr>",
            }
    );

    #endregion Weapon Information
    protected override void Update() {}
    protected override void Attack() {}
    public override void OnEquipped() {
        base.OnEquipped();
        int count = CurrentLevel<5 ? 3 : 6;
        for(int i=0; i<count; i++) {
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