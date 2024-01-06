using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Utility;

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
              $"<nobr>"
            + $"Wield a sword in the direction of aim to damage the monsters.\n"
            + $"\n"
            + $"Damage : {staticDamage[0]}+{damageCoef[0]*100}% / {staticDamage[1]}+{damageCoef[1]*100}% / {staticDamage[2]}+{damageCoef[2]*100}% / {staticDamage[3]}+{damageCoef[3]*100}% / {staticDamage[4]}+{damageCoef[4]*100}%\n"
            + $"Attack Interval : {interval[0]} / {interval[1]} / {interval[2]} / {interval[3]} / {interval[4]}\n"
            + $"Attack Area : {areaScale[0]*100}% / {areaScale[1]*100}% / {areaScale[2]*100}% / {areaScale[3]*100}% / {areaScale[4]*100}%"
            + $"</nobr>"
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "숏소드",
        Description:
              $"<nobr>"
            + $"조준 방향으로 검을 휘둘러 피해를 가합니다.\n"
            + $"\n"
            + $"피해량 : {staticDamage[0]}+{damageCoef[0]*100}% / {staticDamage[1]}+{damageCoef[1]*100}% / {staticDamage[2]}+{damageCoef[2]*100}% / {staticDamage[3]}+{damageCoef[3]*100}% / {staticDamage[4]}+{damageCoef[4]*100}%\n"
            + $"공격 주기 : {interval[0]} / {interval[1]} / {interval[2]} / {interval[3]} / {interval[4]}\n"
            + $"공격 범위 : {areaScale[0]*100}% / {areaScale[1]*100}% / {areaScale[2]*100}% / {areaScale[3]*100}% / {areaScale[4]*100}%"
            + $"</nobr>"
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