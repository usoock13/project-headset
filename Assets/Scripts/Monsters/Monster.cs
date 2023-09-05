using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IDamageable {
    private Character targetCharacter;
    protected Character TargetCharacter {
        get {
            targetCharacter ??= GameManager.instance.Character;
            return targetCharacter;
        }
        set { targetCharacter ??= value; }
    }
    [SerializeField] protected Movement movement;
    [SerializeField] protected StateMachine stateMachine;
    [SerializeField] protected Animator animator;

    [SerializeField] protected float moveSpeed = 5f;
    protected Vector2 targetDirection;
    protected Vector2 MoveVector {
        get { return targetDirection * moveSpeed; }
    }
    public abstract string MonsterType { get; }

    [SerializeField] protected State chaseState = new State("Chase");
    [SerializeField] protected State hitState = new State("Hit");
    [SerializeField] protected State dieState = new State("Die");

    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected float currentHp = 0;
    public bool isArrive { get; private set;} = true;

    #region IDamageable Define
    public abstract void TakeAttackDelay(float amount);
    public abstract void TakeDamage(float amount);
    public abstract void TakeForce(Vector2 force);
    #endregion IDamageable Define
    
    private void Awake() {
        movement ??= GetComponent<Movement>();
        stateMachine ??= GetComponent<StateMachine>();
        animator ??= GetComponent<Animator>();

        InitializeState();
    }
    private void Start() {
        StartCoroutine(UpdateTargetPoint());
    }

    public void InitializeMonster() {
        isArrive = true;
        currentHp = maxHp;
    }
    private void Die() {
        isArrive = false;
    }
    protected abstract void InitializeState();
    private IEnumerator UpdateTargetPoint() {
        while(isArrive) {
            targetDirection = (TargetCharacter.transform.position - transform.position).normalized;
            yield return new WaitForSeconds(.4f);
        }
    }
}