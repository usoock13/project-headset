using System.Collections;
using UnityEngine;

public class WeaponAxe : Weapon {
    [SerializeField] private EffectFlyingAxe projectile;
    private ObjectPooler effectPooler;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] interval = new float[MAX_LEVEL]         {   1.4f,    1.4f,    1.4f,    1.4f,    1.4f,  }; // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]       {   0.9f,    1.2f,    1.2f,    1.7f,    1.7f,  }; // 피해계수
    private float[] projectileScale = new float[MAX_LEVEL]  {   1.0f,    1.0f,    1.5f,    1.5f,    2.0f,  };
    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power;
    public float ProjectileScale => projectileScale[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "벌목 도구";
    public override string Description =>
        level switch {
            // 4       => $"{interval[level]}초에 한 번 조준 방향을 향해 모던 벌목 도구를 던져 적중하는 모든 적에게 {damageCoef[level]*100}%의 피해를 가합니다.",
            2 or 4  => string.Join(System.Environment.NewLine,
                       $"{interval[level]}초에 한 번 조준 방향을 향해 클래식 벌목 도구를 던져 적중하는 모든 적에게 {damageCoef[level]*100}%의 피해를 가합니다.",
                       $"벌목 도구의 크기가 {Mathf.RoundToInt(projectileScale[level] - projectileScale[0]) * 100}% 증가합니다."),

            _       => string.Join(System.Environment.NewLine,
                       $"{interval[level]}초에 한 번 조준 방향을 향해 클래식 벌목 도구를 던져 적중하는 모든 적에게 {damageCoef[level]*100}%의 피해를 가합니다."),
        };
    #endregion Weapon Information

    private void Awake() {
        effectPooler = new ObjectPooler(
            poolingObject: projectile.gameObject,
            onCreated: (GameObject instance) => {
                var effect = instance.GetComponent<EffectFlyingAxe>();
                effect.originWeapon = this;
                effect.onDisapear += (projectile) => {
                    effectPooler.InPool(projectile.gameObject);
                };
            }, 
            parent: this.transform);
    }
    protected override void Attack() {
        effectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        _Character.OnAttack();
    }
}