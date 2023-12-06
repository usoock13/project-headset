using System.Collections;
using UnityEngine;

public class WeaponTentacleBag : Weapon {
    [SerializeField] private EffectSpear spearEffect;
    [SerializeField] private EffectSideSpear sideSpearEffect;
    private ObjectPooler effectPooler;
    private ObjectPooler sideEffectPooler;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]         {   1.2f,    1.2f,    1.0f,    1.0f,    1.0f,  }; // 공격 간격
    private float[] damageCoef = new float[MAX_LEVEL]       {   1.0f,    1.3f,    1.3f,    1.7f,    1.7f,  }; // 피해계수
    private float[] sideDamageCoef = new float[MAX_LEVEL]   {  0.45f,   0.45f,   0.45f,   0.45f,   0.45f,  }; // 추가 피해계수
    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power;
    public float SideDamage => sideDamageCoef[level-1] * _Character.Power;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "장창";
    public override string Description =>
        level switch {
            4 => $"<nobr><color=#f40>{interval[level]}</color>초에 한 번 조준 방향을 향해 창을 여러 번 찔러 넣어 {sideDamageCoef[level]*100}%/{sideDamageCoef[level]*100}%/{damageCoef[level]*100}%</color>의 피해를 가합니다.</nobr>",
            _ => $"<nobr><color=#f40>{interval[level]}</color>초에 한 번 조준 방향을 향해 창을 찔러 넣어 <color=#f40>{damageCoef[level]*100}%</color>의 피해를 가합니다.</nobr>",
        };
    #endregion Weapon Information

    private void Awake() {
        effectPooler = new ObjectPooler(poolingObject: spearEffect.gameObject, parent: this.transform);
        sideEffectPooler = new ObjectPooler(poolingObject: sideSpearEffect.gameObject, parent: this.transform);
    }
    protected override void Attack() {
        StartCoroutine(AttackCoroutine());
        _Character.OnAttack();
    }
    private IEnumerator AttackCoroutine() {
        Vector3 effectPoint = _Character.attackArrow.position;
        Quaternion rot = _Character.attackArrow.rotation;
        if(level >= 5) {
            GameObject sideEffect;
            
            sideEffect = sideEffectPooler.OutPool(effectPoint, rot * Quaternion.Euler(Vector3.forward * 5f));
            sideEffect.transform.Rotate(Vector3.forward * 5f);
            StartCoroutine(InPoolEffect(5f, sideEffect, sideEffectPooler));
            yield return new WaitForSeconds(.10f);
            
            sideEffect = sideEffectPooler.OutPool(effectPoint, rot * Quaternion.Euler(Vector3.forward * -5f));
            sideEffect.transform.Rotate(Vector3.forward * -5f);
            StartCoroutine(InPoolEffect(5f, sideEffect, sideEffectPooler));
            yield return new WaitForSeconds(.10f);
        }
        GameObject mainEffect = effectPooler.OutPool(effectPoint, rot);
        StartCoroutine(InPoolEffect(5f, mainEffect, effectPooler));
    }
    private IEnumerator InPoolEffect(float delay, GameObject effect, ObjectPooler pooler) {
        yield return new WaitForSeconds(delay);
        pooler.InPool(effect);
    }
}