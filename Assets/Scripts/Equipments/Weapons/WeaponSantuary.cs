using System.Collections;
using UnityEngine;

public class WeaponSantuary : Weapon {
    [SerializeField] private EffectSantuary effects;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] staticDamage    = new float[MAX_LEVEL]   { 7f,   12f,   12f,   20f,     25f }; // 고정 피해량
    private float[] healAmount      = new float[MAX_LEVEL]   { 2f,    2f,    3f,    3f,      4f }; // 체력 회복량
    private float[] hittingDelay      = new float[MAX_LEVEL] { 0f,    0f,    0f,    0f,   0.25f }; // 경직 시간
    protected override float AttackInterval => 999;
    public float Damage => staticDamage[level-1];
    public float HealAmount => healAmount[level-1];
    public float HittingDelay => hittingDelay[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "성역";
    public override string Description =>
        level switch {
            >= 4 => $"<nobr>신성한 영역을 펼쳐 0.5초마다 범위 내의 적에게 {staticDamage[level]}의 피해를 가하고 0.25초간 경직시킵니다.\n"
                  + $"이동하지 않으면 2초 걸쳐 범위가 증가하며, 최대치에 도달하면 2초마다 {healAmount[level]}의 체력을 회복합니다.\n"
                  + $"범위는 이동하는 동안 5초에 걸쳐 원래대로 돌아옵니다.\n"
                  + $"이 무기는 공격 속도에 영향을 받지 않습니다.</nobr>",
            _    => $"<nobr>신성한 영역을 펼쳐 0.5초마다 범위 내의 적에게 {staticDamage[level]}의 피해를 가합니다.\n"
                  + $"이동하지 않으면 2초 걸쳐 범위가 증가하며, 최대치에 도달하면 2초마다 {healAmount[level]}의 체력을 회복합니다.\n"
                  + $"범위는 이동하는 동안 5초에 걸쳐 원래대로 돌아옵니다.\n"
                  + $"이 무기는 공격 속도에 영향을 받지 않습니다.</nobr>",
        };
    #endregion Weapon Information
    protected override void Update() {}
    protected override void Attack() {}
    public override void OnEquipped() {
        base.OnEquipped();
        effects.gameObject.SetActive(true);
    }
}