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
        Description: $"<nobr>적과 충돌하면 폭발하여 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 주는 화염구를 <color=#f40>{(level<4 ? 3 : 6)}</color>개 소환합니다.\n"
                   + $"화염구는 캐릭터 주변을 회전하며, 폭발 후 <color=#f40>{interval[NextLevelIndex]}초</color>가 지나면 재생성됩니다."
                   + $"이 무기는 공격 속도에 영향을 받지 않습니다.</nobr>"
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "화염구 저글링",
        Description: $"<nobr>적과 충돌하면 폭발하여 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 주는 화염구를 <color=#f40>{(level<4 ? 3 : 6)}</color>개 소환합니다.\n"
                   + $"화염구는 캐릭터 주변을 회전하며, 폭발 후 <color=#f40>{interval[NextLevelIndex]}초</color>가 지나면 재생성됩니다."
                   + $"이 무기는 공격 속도에 영향을 받지 않습니다.</nobr>"
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