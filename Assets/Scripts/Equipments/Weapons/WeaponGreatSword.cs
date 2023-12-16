using System;
using System.Collections;
using UnityEngine;

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
    public override Sprite Icon => _weaponIcon;
    public override string Name => "그레이트 소드";
    public override string Description =>
            NextLevelIndex switch {
                3 or 4 => $"<nobr><color=#f40>{interval[NextLevelIndex]}초</color>에 한 번 조준 방향을 향해 대검을 휘둘러 적중한 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다."
                        + $"추가로 범위가 <color=#f40>{(areaScale[NextLevelIndex]-1) * 100}%</color> 증가합니다.</nobr>",
                _      => $"<nobr><color=#f40>{interval[NextLevelIndex]}초</color>에 한 번 조준 방향을 향해 대검을 휘둘러 적중한 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.</nobr>"
            };
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