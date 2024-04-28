using System;
using System.Collections;
using UnityEngine;

public class AWeaponAcidFlask : Weapon {    
    [SerializeField] private EffectAwAcidFlask flaskOrigin;
    [SerializeField] private EffectAwAcidCloud cloudOrigin;
    [SerializeField] private AttachmentCorrosion attachmentOrigin;
    public ObjectPooler FlaskPooler { get; private set; }
    public ObjectPooler CloudPooler { get; private set; }
    public ObjectPooler AttachmentPooler { get; private set; }
    
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;

    private float[] staticDamage = new float[MAX_LEVEL] {   8f,    10f,    12f,    14f,    16f, };
    private float[] damageCoef = new float[MAX_LEVEL]   { 0.15f,  0.15f,  0.15f,  0.15f,  0.15f, };

    public readonly float cloudDuration = 5f;
    public readonly float slowAmount = 0.30f;
    public readonly float extraDamageScale = 0.06f;
    public float Damage => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    protected override float AttackInterval => 2.50f;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "The Swamp",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Throw a flask makes acid cloud. Each second the acid cloud gives <color=#f40>Corroded</color> lasting for 5 seoncds to monsters in the cloud."
                   + $"\n"
                   + $"\nAttack Duration : <color=#f40>{AttackInterval}sec</color>"
                   + $"\nCloud Duration : <color=#f40>{cloudDuration}sec</color>"
                   + $"\n<color=#f40>[Corroded]</color>"
                   + $"\nIt reduces the movement speed of monster affected. It can be duplicated up to 5 stack, each stack damages monster and increases all damage monster takes."
                   + $"\nSlow Amount : <color=#f40>{slowAmount*100}%</color>"
                   + $"\nDamage per Stack : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nIncreasing Rate Stack : <color=#f40>{extraDamageScale*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Throw a flask makes acid cloud. Each second the acid cloud gives <color=#f40>Corroded</color> lasting for 5 seoncds to monsters in the cloud."
                   + $"\n"
                   + $"\nAttack Duration : <color=#f40>{AttackInterval}sec</color>"
                   + $"\nCloud Duration : <color=#f40>{cloudDuration}sec</color>"
                   + $"\n<color=#f40>[Corroded]</color>"
                   + $"\nIt reduces the movement speed of monster affected. It can be duplicated up to 5 stack, each stack damages monster and increases all damage monster takes."
                   + $"\nSlow Amount : <color=#f40>{slowAmount*100}%</color>"
                   + $"\nDamage per Stack : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nIncreasing Rate Stack : <color=#f40>{extraDamageScale*100}%</color>"
                   + $"</nobr>"
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "늪",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"산성 구름을 생성하는 플라스크를 던집니다. 산성 구름은 매초 안에 있는 몬스터들에게 5초동안 지속되는 <color=#f40>부식</color>을 부착합니다."
                   + $"\n"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"\n구름 지속시간 : <color=#f40>{cloudDuration}초</color>"
                   + $"\n<color=#f40>[부식]</color>"
                   + $"\n부착된 몬스터는 느려집니다. 최대 5까지 중첩될 수 있으며, 각 중첩은 몬스터에게 피해를 가하고 받는 모든 피해를 증가 시킵니다."
                   + $"\n둔화량 : <color=#f40>{slowAmount*100}%</color>"
                   + $"\n중첩당 피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n중첩당 피해 증가량 : <color=#f40>{extraDamageScale*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"산성 구름을 생성하는 플라스크를 던집니다. 산성 구름은 매초 안에 있는 몬스터들에게 5초동안 지속되는 <color=#f40>부식</color>을 부착합니다."
                   + $"\n"
                   + $"\n공격 주기 : <color=#f40>{AttackInterval}초</color>"
                   + $"\n구름 지속시간 : <color=#f40>{cloudDuration}초</color>"
                   + $"\n<color=#f40>[부식]</color>"
                   + $"\n부착된 몬스터는 느려집니다. 최대 5까지 중첩될 수 있으며, 각 중첩은 몬스터에게 피해를 가하고 받는 모든 피해를 증가 시킵니다."
                   + $"\n둔화량 : <color=#f40>{slowAmount*100}%</color>"
                   + $"\n중첩당 피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n중첩당 피해 증가량 : <color=#f40>{extraDamageScale*100}%</color>"
                   + $"</nobr>"
            }
    );
    #endregion Weapon Information

    private void Awake() {
        FlaskPooler = new ObjectPooler(flaskOrigin.gameObject, parent: this.transform);
        CloudPooler = new ObjectPooler(cloudOrigin.gameObject, parent: this.transform);
        AttachmentPooler = new ObjectPooler(attachmentOrigin.gameObject, parent: this.transform);
    }

    protected override void Attack() {
        FlaskPooler.OutPool(transform.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }

    public override bool Filter() => GameManager.instance.StageManager.Character.level >= 20;
}