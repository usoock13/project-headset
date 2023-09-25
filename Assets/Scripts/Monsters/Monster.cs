using System;
using System.Collections;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
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

    private Coroutine takeHittingDelayCoroutine;

    [SerializeField] protected ObjectPooler ownerPooler = null;

    #region IDamageable Define
    public virtual void TakeDamage(float amount) {
        currentHp -= amount;
        if(currentHp <= 0) {
            stateMachine.ChangeState(dieState);
        }
    }
    public virtual void TakeForce(Vector2 force, float duration=.25f) {
        StartCoroutine(TakeForceCoroutine(force, duration));
    }
    private IEnumerator TakeForceCoroutine(Vector2 force, float duration) {
        float offset = 0;
        float step = 1 / duration;
        Vector2 current = force;
        while(offset <= 1) {
            Vector2 next = Vector2.Lerp(Vector2.zero, force, Mathf.Pow(1-offset, 5));
            movement.MoveToward(current - next);
            current = next;
            offset += Time.deltaTime * step;
            yield return null;
        }
    }
    public virtual void TakeHittingDelay(float second) {
        takeHittingDelayCoroutine = StartCoroutine(TakeHittingDelayCoroutine(second));
    }
    private IEnumerator TakeHittingDelayCoroutine(float second) {
        stateMachine.ChangeState(hitState);
        yield return new WaitForSeconds(second);
        stateMachine.ChangeState(chaseState);
    }
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