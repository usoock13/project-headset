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
                >= 4 => $"<nobr>신성한 영역을 펼쳐 0.5초마다 범위 내의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가하고 0.25초간 경직시킵니다.\n"
                    + $"이 무기는 공격 속도에 영향을 받지 않습니다.</nobr>",
                _    => $"<nobr>신성한 영역을 펼쳐 0.5초마다 범위 내의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.\n"
                    + $"이 무기는 공격 속도에 영향을 받지 않습니다.</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "성역",
        Description:
            NextLevelIndex switch {
                >= 4 => $"<nobr>신성한 영역을 펼쳐 0.5초마다 범위 내의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가하고 0.25초간 경직시킵니다.\n"
                    + $"이 무기는 공격 속도에 영향을 받지 않습니다.</nobr>",
                _    => $"<nobr>신성한 영역을 펼쳐 0.5초마다 범위 내의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.\n"
                    + $"이 무기는 공격 속도에 영향을 받지 않습니다.</nobr>",
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