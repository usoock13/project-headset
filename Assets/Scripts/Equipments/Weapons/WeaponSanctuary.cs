using System.Collections;
using UnityEngine;

public class WeaponSanctuary : Weapon {
    [SerializeField] private EffectSanctuary effects;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] damageCoef = new float[MAX_LEVEL]   { 0.1f,  0.1f,  0.1f,  0.1f,   0.1f }; // 고정 피해량
    private float[] staticDamage = new float[MAX_LEVEL] {  10f,   15f,   20f,   25f,    30f }; // 고정 피해량
    private float[] hittingDelay = new float[MAX_LEVEL] {   0f,    0f,    0f,    0f,  0.25f }; // 경직 시간
    protected override float AttackInterval => 999;
    public float Damage => staticDamage[level-1];
    public float HittingDelay => hittingDelay[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Sanctuary",
        Description:
            NextLevelIndex switch {
                0 => "<nobr>"
                  + $"Spread the holy area and continuous damage monsters in the area."
                  + $"\n"
                  + $"\nDPS : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                  + $"\nStaggering Time : <color=#f40>{hittingDelay[0]}</color>"
                  + $"</nobr>",
                _ => "<nobr>"
                  + $"Spread the holy area and continuous damage monsters in the area."
                  + $"\n"
                  + $"\nDPS : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                  + $"\nStaggering Time : <color=#f40>{hittingDelay[level-1]}</color> > <color=#f40>{hittingDelay[NextLevelIndex]}</color>"
                  + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "성역",
        Description:
            NextLevelIndex switch {
                0 => "<nobr>"
                  + $"신성한 영역을 펼져 범위 안의 몬스터에게 지속적으로 피해를 가합니다."
                  + $"\n"
                  + $"\n초당 피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                  + $"\n경직 시간 : <color=#f40>{hittingDelay[0]}</color>"
                  + $"</nobr>",
                _ => "<nobr>"
                  + $"신성한 영역을 펼져 범위 안의 몬스터에게 지속적으로 피해를 가합니다."
                  + $"\n"
                  + $"\n초당 피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                  + $"\n경직 시간 : <color=#f40>{hittingDelay[level-1]}</color> > <color=#f40>{hittingDelay[NextLevelIndex]}</color>"
                  + $"</nobr>"
            }
    );
    #endregion Weapon Information
    
    protected override void Update() {}
    protected override void Attack() {}
    public override void OnEquipped() {
        base.OnEquipped();
        effects.gameObject.SetActive(true);
    }
}