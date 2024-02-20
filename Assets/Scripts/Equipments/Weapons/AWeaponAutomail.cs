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

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "The Judgement",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Punch fast with the iron fist to damage monsters. Each hit increases <color=#f40>Overload</color> stack up to 100."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>0.2sec</color>"
                   + $"\n<color=#f40>[Overload]</color>"
                   + $"\nIncrease punch speed up to 300%. When reach to 100 stacks, after a while cause explosion and damage monsters around the character. After explosion, punch need rest."
                   + $"\nExplosion Damage : <color=#f40>{explosionStaticDamage[0]}+{explosionDamageCoef[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Punch fast with the iron fist to damage monsters. Each hit increases <color=#f40>Overload</color> stack up to 100."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nAttack Interval : <color=#f40>0.2sec</color>"
                   + $"\n<color=#f40>[Overload]</color>"
                   + $"\nIncrease punch speed up to 300%. When reach to 100 stacks, after a while cause explosion and damage monsters around the character. After explosion, punch need rest."
                   + $"\nExplosion Damage : <color=#f40>{explosionStaticDamage[level-1]}+{explosionDamageCoef[level-1]*100}%</color> > <color=#f40>{explosionStaticDamage[NextLevelIndex]}+{explosionDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "정의 집행",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"빠르게 강철 주먹 날려 몬스터에게 피해를 가합니다. 주먹이 적중하면 <color=#f40>과부화</color>가 중첩되며, 최대 100까지 중첩됩니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>0.2초</color>"
                   + $"\n<color=#f40>[과부화]</color>"
                   + $"\n펀치의 속도를 최대 300%까지 증가시킵니다. 100 중첩에 도달하면 잠시 뒤 폭발하여 캐릭터 주변의 몬스터에게 피해를 가합니다. 폭발 이후 주먹에게 휴식이 필요합니다."
                   + $"\n폭발 피해량 : <color=#f40>{explosionStaticDamage[0]}+{explosionDamageCoef[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"빠르게 강철 주먹 날려 몬스터에게 피해를 가합니다. 주먹이 적중하면 <color=#f40>과부화</color>가 중첩되며, 최대 100까지 중첩됩니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n공격 주기 : <color=#f40>0.2초</color>"
                   + $"\n<color=#f40>[과부화]</color>"
                   + $"\n펀치의 속도를 최대 300%까지 증가시킵니다. 100 중첩에 도달하면 잠시 뒤 폭발하여 캐릭터 주변의 몬스터에게 피해를 가합니다. 폭발 이후 주먹에게 휴식이 필요합니다."
                   + $"\n폭발 피해량 : <color=#f40>{explosionStaticDamage[level-1]}+{explosionDamageCoef[level-1]*100}%</color> > <color=#f40>{explosionStaticDamage[NextLevelIndex]}+{explosionDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
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
                monster.TakeStagger(2f);
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