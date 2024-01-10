using System;
using System.Collections;
using UnityEngine;

public class WeaponMaliciousFlask : Weapon {
    [SerializeField] private EffectMaliciousFlask flaskEffect;
    [SerializeField] private AttachmentSlowPoison attachmentSlowPoison;
    private ObjectPooler effectPooler;
    private ObjectPooler attachmentPooler;
    public AttachmentSlowPoison AfterAttahcment => attachmentPooler.OutPool().GetComponent<AttachmentSlowPoison>();
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]     {  2.50f,   2.50f,   2.50f,   2.50f,  2.50f };  // 공격 간격
    private float[] staticDamage = new float[MAX_LEVEL] {  7.00f,   9.00f,  11.00f,  13.00f, 15.00f };  // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]   {  0.10f,   0.10f,   0.10f,   0.15f,  0.20f };  // 피해계수
    private float[] slowAmount = new float[MAX_LEVEL]   {  0.10f,   0.15f,   0.20f,   0.25f,  0.30f };  // 둔화율
    protected override float AttackInterval => interval[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Slow Poison",
        Description:
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"Throw a flask in the direction of aim. After collide monster or reach to maximum distance, the flask breaks and attach 'Slow Poison' to the monsters that in area."
                   + $"\n"
                   + $"\nDamage per Second : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[0]}sec</color>"
                   + $"\nSlow Amount : <color=#f40>{slowAmount[0]*100}%</color>"
                   + $"\nDuration : <color=#f40>3sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Throw a flask in the direction of aim. After collide monster or reach to maximum distance, the flask breaks and attach 'Slow Poison' to the monsters that in area."
                   + $"\n"
                   + $"\nDamage per Second : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\nSlow Amount : <color=#f40>{slowAmount[level-1]*100}%</color> > <color=#f40>{slowAmount[NextLevelIndex]*100}%</color>"
                   + $"\nDuration : <color=#f40>3sec</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "둔화독",
        Description:
            CurrentLevel switch {
                0 => $"<nobr>"
                   + $"조준 방향으로 플라스크를 던집니다. 플라스크는 몬스터와 충돌하거나 사거리 끝에 도달하면 깨지며 범위 내의 적에게 '둔화 독'을 부착합니다."
                   + $"\n"
                   + $"\n초당 피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[0]}sec</color>"
                   + $"\n둔화량 : <color=#f40>{slowAmount[0]*100}%</color>"
                   + $"\n지속 시간 : <color=#f40>3sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"조준 방향으로 플라스크를 던집니다. 플라스크는 몬스터와 충돌하거나 사거리 끝에 도달하면 깨지며 범위 내의 적에게 '둔화 독'을 부착합니다."
                   + $"\n"
                   + $"\n초당 피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>{interval[level-1]}sec</color> > <color=#f40>{interval[NextLevelIndex]}sec</color>"
                   + $"\n둔화량 : <color=#f40>{slowAmount[level-1]*100}%</color> > <color=#f40>{slowAmount[NextLevelIndex]*100}%</color>"
                   + $"\n지속 시간 : <color=#f40>3sec</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information

    private void Awake() {
        effectPooler = new ObjectPooler(
            poolingObject: flaskEffect.gameObject,
            parent: this.transform,
            onCreated: (gobj) => {
                gobj.GetComponent<EffectMaliciousFlask>().onDisappear += (projectile) => {
                    StartCoroutine(InPoolCoroutine(effectPooler, projectile.gameObject));
                };
        });
        attachmentPooler = new ObjectPooler(
            poolingObject: attachmentSlowPoison.gameObject,
            parent: this.transform,
            onCreated: (gobj) => {
                gobj.GetComponent<AttachmentSlowPoison>().onDetached += (attachment) => {
                    attachmentPooler.InPool(gobj);
                };
            });
    }
    private IEnumerator InPoolCoroutine(ObjectPooler pooler, GameObject effect) {
        yield return new WaitForSeconds(5f);
        pooler.InPool(effect);
    }
    protected override void Attack() {
        GameObject flaskInstance = effectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        var effect = flaskInstance.GetComponent<EffectMaliciousFlask>();
        effect.originWeapon = this;
        _Character.OnAttack();
    }
    public float GetDamage(int level) {
        return staticDamage[level-1] + (_Character.Power * damageCoef[level-1]);
    }
    public float GetSlowAmount(int level) {
        return slowAmount[level-1];
    }
}