using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(StateMachine))]
public abstract class Monster : MonoBehaviour, IDamageable, IAttachmentsTakeable {
    private Character targetCharacter;
    protected Character TargetCharacter {
        get {
            targetCharacter ??= GameManager.instance.Character;
            return targetCharacter;
        }
        set { targetCharacter ??= value; }
    }
    private StageManager _StageManager => GameManager.instance.StageManager;
    
    public GameObject GameObject => this.gameObject;

    [SerializeField] protected Movement movement;
    [SerializeField] protected StateMachine stateMachine;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteAnimator spriteAnimator;
    [SerializeField] new protected Collider2D collider2D;
    [SerializeField] new protected Rigidbody2D rigidbody2D;

    [SerializeField] protected float defaultMoveSpeed = 5f;
    private Func<Monster, float> speedModifier;
    public float MoveSpeed { get {
        float final = defaultMoveSpeed;
        Delegate[] m = speedModifier?.GetInvocationList();
        if(m != null)
            for(int i=0; i<m.Length; i++) {
                Func<Monster, float> f = (Func<Monster, float>) m[i];
                final *= f(this);
            }
        return final>0 ? final : 0;
    }}
    protected Vector2 MoveVector => targetDirection * MoveSpeed;

    protected Vector2 targetDirection;
    public abstract string MonsterType { get; }

    #region States
    [SerializeField] public State chaseState { get; protected set; } = new State("Chase");
    [SerializeField] public State hitState   { get; protected set; } = new State("Hit");
    [SerializeField] public State dieState   { get; protected set; } = new State("Die");
    #endregion States

    #region Status
    public bool isArrive { get; protected set;} = true;
    [SerializeField] private float defaultToughness = 1f;
    [SerializeField] private float Toughness => defaultToughness > 1  ? defaultToughness  :  1;
    [SerializeField] protected float maxHp = 100;
    protected float currentHp = 0;
    public UnityAction<Monster> onDie;
    #endregion Status

    private List<Attachment> havingAttachment = new List<Attachment>();
    public List<Attachment> H => havingAttachment;

    [SerializeField] protected int givingExp = 10;
    [SerializeField] protected int givingKeso = 20;

    protected Coroutine takeAttackDelayCoroutine;
    protected Coroutine takeForceCoroutine;
    private Coroutine updateTargetDirectionCoroutine;

    protected ObjectPooler ownerPooler = null;

    #region IDamageable Implements
    // When call these three methods, keep below order.
    /* 
        monster.TakeDamage(Damage);
        monster.TakeHittingDelay(hittingDelay);
        monster.TakeForce(transform.up * 1f, hittingDelay);
        
    */
    public virtual void TakeDamage(float amount) {
        currentHp -= amount;
        if(currentHp <= 0)
            stateMachine.ChangeState(dieState);
        _StageManager.PrintDamageNumber(transform.position, ((int) amount).ToString());
    }


    public virtual void TakeAttackDelay(float second) {
        float reduced = second / Toughness;
        if(reduced > 0.1f) {
            takeAttackDelayCoroutine = StartCoroutine(TakeAttackDelayCoroutine(reduced));
        }
    }

    private IEnumerator TakeAttackDelayCoroutine(float second) {
        stateMachine.ChangeState(hitState);
        yield return new WaitForSeconds(second);
        stateMachine.ChangeState(chaseState);
    }


    public virtual void TakeForce(Vector2 force, float duration=.25f) {
        force = Toughness == 0  ?  force  :  force / Toughness;
        duration = Toughness == 0  ?  duration  :  duration / Toughness;
        takeForceCoroutine = StartCoroutine(TakeForceCoroutine(force, duration));
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
    #endregion IDamageable Implements
    
    protected void Awake() {
        movement ??= GetComponent<Movement>();
        stateMachine ??= GetComponent<StateMachine>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        spriteAnimator ??= GetComponent<SpriteAnimator>();
        collider2D ??= GetComponent<Collider2D>();
        rigidbody2D ??= GetComponent<Rigidbody2D>();

        InitializeStates();
    }
    protected void Start() {}
    protected void OnEnable() {
        OnSpawn();
    }
    public void OnSpawn() {
        isArrive = true;
        currentHp = maxHp;
        rigidbody2D.simulated = true;
        ownerPooler = GameManager.instance.StageManager.ScenarioDirector.monsterPoolerMap[this.MonsterType];
        updateTargetDirectionCoroutine = StartCoroutine(UpdateTargetPoint());
        stateMachine.isMuted = false;
        stateMachine.ChangeState(chaseState);
    }
    protected virtual void OnDie() {
        isArrive = false;
        rigidbody2D.simulated = false;
        onDie?.Invoke(this);
        StopCoroutine(updateTargetDirectionCoroutine);
        ClearAttachments();
        _StageManager.OnMonsterDie(this);
        DropExp();
        DropPotion();
        DropKeso();
    }
    protected void DropExp() {
        _StageManager.CreateExp(transform.position, givingExp);
    }
    protected void DropPotion() {
        float ratio = 0.02f;
        float next = UnityEngine.Random.Range(0f, 1f);
        if(next < ratio)
            _StageManager.CreateMeat(transform.position);
    }
    protected void DropKeso() {
        float ratio = 0.25f;
        float next = UnityEngine.Random.Range(0f, 1f);
        int amount = (int)(givingKeso * UnityEngine.Random.Range(0.8f, 1.2f));
        if(next < ratio)
            _StageManager.CreateKeso(transform.position, amount);
    }

    protected virtual void InitializeStates() {
        hitState.onActive += (State prev) => {
            if(takeForceCoroutine != null)
                StopCoroutine(takeForceCoroutine);
        };

        hitState.onInactive += (State next) => {
            StopCoroutine(takeAttackDelayCoroutine);
        };

        dieState.onActive += (State prev) => {
            isArrive = false;
            stateMachine.isMuted = true;
            OnDie();
        };
    }

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

    #region IAttachmentsTakeable Implements
    public void TakeAttachment(Attachment attachment) {
        attachment.transform.SetParent(this.transform);
        attachment.transform.localPosition = Vector2.zero;
        havingAttachment.Add(attachment);
        attachment.OnAttached(this);
    }
    public void ReleaseAttachment(Attachment attachment) {
        havingAttachment.Remove(attachment);
        attachment.OnDetached(this);
    }
    public bool TryGetAttachment(string attachmentType, out Attachment attachment) {
        attachment = havingAttachment.Find((item) => {
            return attachmentType == item.AttachmentType;
        });
        return attachment==null ? false : true;
    }
    #endregion IAttachmentsTakeable Implements
    
    private void ClearAttachments() {
        for(int i=0; i<havingAttachment.Count; i++) {
            havingAttachment[i].OnDetached(this);
        }
        havingAttachment = new List<Attachment>();
    }
    public void AddSpeedModifier(Func<Monster, float> modifier) {
        this.speedModifier += modifier;
        spriteAnimator.Animator.SetFloat("Moving Scale", MoveSpeed / defaultMoveSpeed);
    }
    public void RemoveSpeedModifier(Func<Monster, float> modifier) {
        this.speedModifier -= modifier;
        spriteAnimator.Animator.SetFloat("Moving Scale", MoveSpeed / defaultMoveSpeed);
    }
}