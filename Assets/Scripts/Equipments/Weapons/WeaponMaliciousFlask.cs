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
    public override Sprite Icon => _weaponIcon;
    public override string Name => "둔화독";
    public override string Description => 
        NextLevelIndex switch {
            _ =>$"<nobr><color=#f40>{interval[NextLevelIndex]}</color>초에 한 번 조준 방향으로 둔화독으로 가득찬 약병을 던집니다."
              + $"약병은 적에게 닿거나 사거리 끝에 도달하면 폭발하여 좁은 범위에 적을 중독시켜 3초에 걸쳐 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.</nobr>"
        };
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