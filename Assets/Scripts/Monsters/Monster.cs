using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(StateMachine))]
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
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteAnimator spriteAnimator;
    [SerializeField] new protected Collider2D collider2D;
    [SerializeField] new protected Rigidbody2D rigidbody2D;

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
    public bool isArrive { get; protected set;} = true;
    public UnityAction<Monster> onDie;

    [SerializeField] protected ObjectPooler ownerPooler = null;

    #region IDamageable Define
    public abstract void TakeHittingDelay(float amount);
    public abstract void TakeDamage(float amount);
    public abstract void TakeForce(Vector2 force);
    #endregion IDamageable Define
    
    protected void Awake() {
        movement ??= GetComponent<Movement>();
        stateMachine ??= GetComponent<StateMachine>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        spriteAnimator ??= GetComponent<SpriteAnimator>();
        collider2D ??= GetComponent<Collider2D>();
        rigidbody2D ??= GetComponent<Rigidbody2D>();

        InitializeStates();
    }
    protected void Start() {
        StartCoroutine(UpdateTargetPoint());
    }
    protected void OnEnable() {
        OnSpawn();
    }
    public void OnSpawn() {
        isArrive = true;
        currentHp = maxHp;
        rigidbody2D.simulated = true;
        ownerPooler = GameManager.instance.StageManager.ScenarioDirector.monsterPoolerMap[this.MonsterType];
        stateMachine.ChangeState(chaseState);
    }
    protected virtual void OnDie() {
        isArrive = false;
        rigidbody2D.simulated = false;
        onDie?.Invoke(this);
    }
    protected abstract void InitializeStates();
    private IEnumerator UpdateTargetPoint() {
        while(isArrive) {
            targetDirection = (TargetCharacter.transform.position - transform.position).normalized;
            yield return new WaitForSeconds(.4f);
        }
    }
    protected void LookAt2D(float x) {
        if(x!=0)
            spriteRenderer.flipX = x>0 ? true : false;
    }
}