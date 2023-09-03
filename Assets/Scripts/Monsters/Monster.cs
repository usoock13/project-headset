using Unity.Burst.Intrinsics;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IDamageable {
    protected Character targetCharacter;

    [SerializeField] protected Movement movement;
    [SerializeField] protected StateMachine stateMachine;
    [SerializeField] protected Animator animator;

    [SerializeField] protected float moveSpeed = 5f;
    protected Vector2 TargetDirection {
        get { return (targetCharacter.transform.position - transform.position).normalized; }
    }
    protected Vector2 MoveVector {
        get { return TargetDirection * moveSpeed; }
    }

    [SerializeField] protected State chaseState = new State("Chase");
    [SerializeField] protected State hidState = new State("Hit");
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
    }

    private void InitializeMonster() {
        isArrive = true;
        currentHp = maxHp;
    }
    private void Die() {
        isArrive = false;
    }
    public abstract void ChaseCharacter(Character target);
}