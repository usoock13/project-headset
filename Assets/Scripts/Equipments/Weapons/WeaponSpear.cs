using System.Collections;
using UnityEngine;

public class WeaponSpear : Weapon {
    [SerializeField] private EffectSpear effectOrigin;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]         {   1.2f,    1.2f,    1.0f,    1.0f,    1.0f,  }; // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]       {   0.5f,    0.5f,    0.5f,    0.5f,    0.5f,  }; // 피해 계수
    private float[] staticDamage = new float[MAX_LEVEL]     {    15f,     15f,     30f,     30f,     50f,  }; // 고정 피해량
    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Long Spear",
        Description:
            NextLevelIndex switch {
                _ => $"<nobr><color=#f40>{interval[NextLevelIndex]}</color>초에 한 번 조준 방향을 향해 창을 찔러 넣어 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "장창",
        Description:
            NextLevelIndex switch {
                _ => $"<nobr><color=#f40>{interval[NextLevelIndex]}</color>초에 한 번 조준 방향을 향해 창을 찔러 넣어 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.</nobr>",
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
}