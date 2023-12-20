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

    private float[] staticDamage = new float[MAX_LEVEL] {   10f,    13f,    16f,    19f,    22f, };
    private float[] damageCoef = new float[MAX_LEVEL]   { 0.20f,  0.20f,  0.20f,  0.20f,  0.20f, };

    public readonly float cloudDuration = 8f;
    public readonly float slowAmount = 0.60f;
    public readonly float extraDamageScale = 0.06f;
    public float Damage => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    protected override float AttackInterval => 1.25f;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "산성 플라스크";
    public override string Description => 
                $"<nobr><color=#f40>{AttackInterval}</color>초에 한 번 조준 방향으로 산성 구름을 생성하는 약병을 던집니다."
              + $"약병은 적에게 닿거나 사거리 끝에 도달하면 폭발하여 {cloudDuration}초동안 유지되는 큰 산성 구름을 생성합니다. 산성 구름은 적에게 매초, 5회까지 중첩되는 <color=#f40>부식</color> 상태를 부여합니다.\n"
              + $"<color=#f40><i>부식 상태의 적은 속도가 {slowAmount*100}% 감소하며, 중첩마다 받는 피해가 {extraDamageScale*100}% 증가하고 매초{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 입습니다.</i></nobr>";
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
}