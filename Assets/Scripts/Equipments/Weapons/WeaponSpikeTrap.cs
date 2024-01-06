using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class WeaponSpikeTrap : Weapon {
    [SerializeField] private EffectSpikeTrap trapEffect;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] damageCoef = new float[MAX_LEVEL]       {   0.25f,   0.30f,   0.35f,   0.40f,   0.45f,  }; // 피해 계수
    private float[] staticDamage = new float[MAX_LEVEL]     {     10f,     13f,     16f,     19f,     22f,  }; // 고정 피해량
    private float[] trapScale = new float[MAX_LEVEL]        {      1f,      1f,      1f,    1.2f,    1.5f,  }; // 덫 크기
    protected override float AttackInterval => 1f;
    private float countdownDuringDodge = 0;
    private float intervalDuringDodge = .1f;

    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float Duration => 10f;
    public float HittingDelay => 1f;
    public float TrapScale => trapScale[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Caltrop",
        Description:
            NextLevelIndex switch {
                3 or 4 => $"<nobr>이동하면 바닥에 쇠못덫을 설치합니다. 덫은 밟은 적에게 <color=#ff4400>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고 1초간 경직시킨 뒤 사라집니다.\n"
                        + $"회피하면 경로에 덫을 설치하며 이동합니다.</nobr>",
                _      => $"<nobr>이동하면 바닥에 쇠못덫을 설치합니다. 덫은 밟은 적에게 <color=#ff4400>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고 1초간 경직시킨 뒤 사라집니다.</nobr>"
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "마름쇠",
        Description:
            NextLevelIndex switch {
                3 or 4 => $"<nobr>이동하면 바닥에 쇠못덫을 설치합니다. 덫은 밟은 적에게 <color=#ff4400>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고 1초간 경직시킨 뒤 사라집니다.\n"
                        + $"회피하면 경로에 덫을 설치하며 이동합니다.</nobr>",
                _      => $"<nobr>이동하면 바닥에 쇠못덫을 설치합니다. 덫은 밟은 적에게 <color=#ff4400>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex] * 100}%</color>의 피해를 가하고 1초간 경직시킨 뒤 사라집니다.</nobr>"
            }
    );
    #endregion Weapon Information

    protected override void Update() {
        if(!_Character.CurrentState.Compare(_Character.walkState))
            return;
        if(currentTime > 0) {
            currentTime -= Time.deltaTime;
        } else {
            currentTime += AttackInterval;
            Attack();
        };
    }
    public override void OnGotten() {
        base.OnGotten();
        _Character.dodgeState.onActive += OnDodge;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.dodgeState.onActive -= OnDodge;
    }
    private IEnumerator InstallationDuringDodge() {
        Attack();
        while(_Character.CurrentState.Compare(_Character.dodgeState)) {
            float scale = _Character.MoveSpeed / _Character.DefaultMoveSpeed;
            countdownDuringDodge += Time.deltaTime * scale;
            if(countdownDuringDodge >= intervalDuringDodge) {
                countdownDuringDodge -= intervalDuringDodge;
                Attack();
            }
            yield return null;
        }
    }
    protected void Awake() {
        EffectPooler = new ObjectPooler(
            poolingObject: trapEffect.gameObject,
            parent: this.transform,
            count: 100,
            restoreCount: 20
        );
    }
    protected override void Attack() {
        var instance = EffectPooler.OutPool(transform.position, Quaternion.identity);
        instance.transform.localScale = Vector3.one * TrapScale;
        _Character.OnAttack();
    }
    private void OnDodge(State prev) {
        if(level >= 4)
            StartCoroutine(InstallationDuringDodge());
    }
}