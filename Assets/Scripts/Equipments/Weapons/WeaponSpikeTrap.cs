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
    protected override float AttackInterval => 5f;

    public float DamagePerSecond => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public float Duration => 5f;
    public float HittingDelay => 0.1f;
    public float TrapScale => trapScale[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Caltrop",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"During the character move, set up the trap which damage monsters on that."
                   + $"\nThe faster character's movement is, the faster to set up the trap."
                   + $"\n"
                   + $"\nDPS : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nDuration : <color=#f40>{Duration}sec</color>"
                   + $"\nTrap Scale : <color=#f40>{trapScale[0]*100}%</color>"
                   + $"</nobr>",
                4 => $"<nobr>"
                   + $"During the character move, set up the trap which damage monsters on that."
                   + $"\nThe faster character's movement is, the faster to set up the trap."
                   + $"\n<color=#f40>Now character can does dodge to set up the several traps!</color>"
                   + $"\n"
                   + $"\nDPS : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nDuration : <color=#f40>{Duration}sec</color>"
                   + $"\nTrap Scale : <color=#f40>{trapScale[level-1]*100}%</color> > <color=#f40>{trapScale[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"During the character move, set up the trap which damage monsters on that."
                   + $"\nThe faster character's movement is, the faster to set up the trap."
                   + $"\n"
                   + $"\nDPS : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nDuration : <color=#f40>{Duration}sec</color>"
                   + $"\nTrap Scale : <color=#f40>{trapScale[level-1]*100}%</color> > <color=#f40>{trapScale[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "마름쇠",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터가 이동하는 동안 덫을 설치하여 덫 위의 몬스터에게 피해를 가합니다."
                   + $"\n캐릭터의 이동속도가 빨라지면 덫을 설치하는 속도가 빨라집니다."
                   + $"\n"
                   + $"\n초당 피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n지속시간 : <color=#f40>{Duration}초</color>"
                   + $"\n덫 크기 : <color=#f40>{trapScale[0]*100}%</color>"
                   + $"</nobr>",
                4 => $"<nobr>"
                   + $"캐릭터가 이동하는 동안 덫을 설치하여 덫 위의 몬스터에게 피해를 가합니다."
                   + $"\n캐릭터의 이동속도가 빨라지면 덫을 설치하는 속도가 빨라집니다."
                   + $"\n<color=#f40>이제 회피를 사용하여 덫을 여럿 설치할 수 있습니다!</color>"
                   + $"\n"
                   + $"\n초당 피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n지속시간 : <color=#f40>{Duration}초</color>"
                   + $"\n덫 크기 : <color=#f40>{trapScale[level-1]*100}%</color> > <color=#f40>{trapScale[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터가 이동하는 동안 덫을 설치하여 덫 위의 몬스터에게 피해를 가합니다."
                   + $"\n캐릭터의 이동속도가 빨라지면 덫을 설치하는 속도가 빨라집니다."
                   + $"\n"
                   + $"\n초당 피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n지속시간 : <color=#f40>{Duration}초</color>"
                   + $"\n덫 크기 : <color=#f40>{trapScale[level-1]*100}%</color> > <color=#f40>{trapScale[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information

    protected override void Update() {
        if(!_Character.CurrentState.Compare(_Character.walkState))
            return;
        if(currentTime > 0) {
            currentTime -= Time.deltaTime * _Character.MoveSpeed * _Character.AttackSpeed;
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
    private void InstallationDuringDodge() {
        StartCoroutine(ThrowTrap(72*0));
        StartCoroutine(ThrowTrap(72*1));
        StartCoroutine(ThrowTrap(72*2));
        StartCoroutine(ThrowTrap(72*3));
        StartCoroutine(ThrowTrap(72*4));
        _Character.OnAttack();
    }
    private IEnumerator ThrowTrap(float angle) {
        var trap = EffectPooler.OutPool(transform.position, Quaternion.identity);
        trap.transform.Rotate(Vector3.forward, angle);

        Vector2 start = trap.transform.position;
        Vector2 end = trap.transform.position + trap.transform.up * 1.5f;
        float offset = 0;
        while(offset < 1) {
            offset += Time.deltaTime * 3f;
            trap.transform.position = Vector2.Lerp(start, end, 1-Mathf.Pow(1-offset, 3));
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
        EffectPooler.OutPool(transform.position, Quaternion.identity);
        _Character.OnAttack();
    }
    private void OnDodge(State prev) {
        if(level >= 4)
            InstallationDuringDodge();
    }
}