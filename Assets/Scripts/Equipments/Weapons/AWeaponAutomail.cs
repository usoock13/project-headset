using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AWeaponAutomail : Weapon {
    [SerializeField] private EffectAwMetalPunch punchEffect;
    public ObjectPooler EffectPooler { get; private set; }

    [SerializeField] private LayerMask targetLayer = 1<<8;
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] staticDamage = new float[MAX_LEVEL]          {  25f,     30f,     35f,     40f,     45f }; // 고정 피해량
    private float[] damageCoef = new float[MAX_LEVEL]            { 0.3f,    0.3f,    0.3f,    0.3f,    0.3f }; // 피해 계수
    private float[] explosionStaticDamage = new float[MAX_LEVEL] { 500f,    750f,   1000f,   1250f,   1500f }; // 피해 계수
    private float[] explosionDamageCoef = new float[MAX_LEVEL]   { 3.0f,    3.0f,    3.0f,    3.0f,    3.0f }; // 피해 계수
    protected override float AttackInterval => 0.2f / (1 + 0.02f * overload);
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float ExplosionDamage => explosionDamageCoef[level-1] * _Character.Power + explosionStaticDamage[level-1];
    #endregion Weapon Status

    private bool isCharging = false;
    private bool isResting = false;
    private readonly int maxOverLoad = 100;
    private int overload = 0;
    private int Overload {
        get => overload;
        set {
            overload = value;
            extraInformation = $"{overload:0}";
            GameManager.instance.StageManager.StageUIManager.UpdateWeaponList();
        }
    }

    [SerializeField] ParticleSystem chargingEffect;
    [SerializeField] ParticleSystem explosionEffect;

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "오토메일";
    public override string Description => 
        NextLevelIndex switch {
            _ => $"<nobr>강철 주먹을 마구 내질러 적중한 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가하고 중첩 1을 얻습니다.\n중첩 1당 주먹의 공격 속도가 2% 증가하고 100이 되면 잠시 뒤 폭발을 일으켜 주변 넓의 범위의 적에게 <color=#f40>{explosionDamageCoef[NextLevelIndex]}+{explosionDamageCoef[NextLevelIndex]*100}%</color>의 피해를 가하고 중첩을 모두 잃으며, 잠깐동안 주먹을 진정시키는 시간을 갖습니다.</nobr>",
        };
    #endregion Weapon Information

    private void Awake() {
        EffectPooler = new ObjectPooler(poolingObject: punchEffect.gameObject, parent: this.transform);
    }
    protected override void Attack() {
        if(isResting)
            return;

        var effect = EffectPooler.OutPool(_Character.attackArrow.position, _Character.attackArrow.rotation);
        effect.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(-30f, 30f));
        StartCoroutine(InPoolEffect(effect));
        _Character.OnAttack();
    }

    private IEnumerator InPoolEffect(GameObject effect) {
        yield return new WaitForSeconds(3f);
        EffectPooler.InPool(effect);
    }

    public void OnAttackMonster() {
        if(isCharging || isResting)
            return;

        Overload ++;
        if(Overload >= 100) {
            StartCoroutine(ExplosionCoroutine());
        }
    }

    private IEnumerator ExplosionCoroutine() {
        isCharging = true;

        chargingEffect.Play();
        yield return new WaitForSeconds(2.5f);
        explosionEffect.Play();
        var inners = Physics2D.OverlapCircleAll(transform.position, 5f, targetLayer);
        for(int i=0; i<inners.Length; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(ExplosionDamage);
                monster.TakeAttackDelay(2f);
                monster.TakeForce((inners[i].transform.position - transform.position).normalized * 15f, 2f);
                _Character.OnAttackMonster(monster);
            }
        }
        isResting = true;
        
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime * 0.2f * _Character.AttackSpeed;
            Overload = (int) Mathf.Lerp(maxOverLoad, 0, offset);
            yield return null;
        }
        Overload = 0;
        isResting = false;
        isCharging = false;
    }

    public override void OnGotten() {
        base.OnGotten();
        Overload = 0;
    }
}