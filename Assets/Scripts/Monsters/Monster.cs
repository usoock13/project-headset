using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(StateMachine))]
public abstract class Monster : MonoBehaviour, IDamageable, IAttachmentsTakeable {
    private Transform targetCharacter;
    public Transform TargetCharacter {
        get {
            targetCharacter ??= GameManager.instance.Character.transform;
            return targetCharacter;
        }
        set { targetCharacter = value; }
    }
    protected StageManager _StageManager => GameManager.instance.StageManager;
    
    public GameObject GameObject => this.gameObject;

    [SerializeField] protected Movement movement;
    [SerializeField] protected StateMachine stateMachine;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteAnimator spriteAnimator;
    [SerializeField] new protected Collider2D collider2D;
    [SerializeField] new protected Rigidbody2D rigidbody2D;

    protected Material material;
    [SerializeField] private SpriteColorManager colorManager;
    public SpriteColorManager ColorManager => colorManager;

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

    public Func<Monster, float> extraDamageScale;
    private float DamageScale { get {
        float final = 1;
        Delegate[] scales = extraDamageScale?.GetInvocationList();
        if(scales != null)
            for(int i=0; i<scales.Length; i++)
                final += ((Func<Monster, float>) scales[i])?.Invoke(this) ?? 0;
        return final;
    }}

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
    [SerializeField] private float Toughness { get {
        float final = defaultToughness * _StageManager.StageLevel;
        final = final > 1  ?  final  :  1;
        return final;
    }}
    [SerializeField] protected float defaultHp = 100;
    [SerializeField] protected float maxHp = 100;
    protected float currentHp = 0;
    public UnityAction<Monster> onDie;
    #endregion Status

    private List<Attachment> havingAttachment = new List<Attachment>();
    public List<Attachment> HavingAttackment => havingAttachment;

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
        monster.TakeAttackDelay(hittingDelay);
        monster.TakeForce(transform.up * 1f, hittingDelay);
        
    */
    public virtual void TakeDamage(float amount) {
        float damage = amount * DamageScale;
        currentHp -= damage;
        if(currentHp <= 0)
            stateMachine.ChangeState(dieState);
        _StageManager.PrintDamageNumber(transform.position, ((int) damage).ToString());
    }


    public virtual void TakeStagger(float second) {
        float reduced = second / Toughness;
        if(reduced > 0.1f) {
            takeAttackDelayCoroutine = StartCoroutine(TakeAttackDelayCoroutine(reduced));
        } else {
            StartCoroutine(TakeBitDelayCoroutine());
        }
    }

    private IEnumerator TakeAttackDelayCoroutine(float second) {
        stateMachine.ChangeState(hitState);
        yield return new WaitForSeconds(second);
        stateMachine.ChangeState(chaseState);
    }
    
    protected IEnumerator TakeBitDelayCoroutine() {
        material?.SetFloat("_Hit_Effect_Scale", 0.3f);
        yield return new WaitForSeconds(0.1f);
        material?.SetFloat("_Hit_Effect_Scale", 0);
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
            movement.Translate(current - next);
            current = next;
            offset += Time.deltaTime * step;
            yield return null;
        }
    }
    #endregion IDamageable Implements
    
    #region Unity events
    protected virtual void Awake() {
        movement ??= GetComponent<Movement>();
        stateMachine ??= GetComponent<StateMachine>();
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        spriteAnimator ??= GetComponent<SpriteAnimator>();
        collider2D ??= GetComponent<Collider2D>();
        rigidbody2D ??= GetComponent<Rigidbody2D>();
        colorManager ??= GetComponent<SpriteColorManager>();

        InitializeStates();
    }
    protected void Start() {
        _StageManager.onChageStageLevel += IncreaseStageLevelHandler;
        material = spriteRenderer?.material;
    }

    protected void OnEnable() {
        OnSpawn();
    }
    #endregion Unity events

    public virtual void OnSpawn() {
        isArrive = true;
        maxHp = defaultHp * _StageManager.StageLevel;
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
        DropMeat();
        DropKeso();
    }

    protected void DropExp() {
        _StageManager.CreateExp(transform.position, (int)(givingExp * _StageManager.StageLevel));
    }

    protected void DropMeat() {
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
            _StageManager.CreateKeso(transform.position, (int)(amount * _StageManager.StageLevel));
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
            material?.SetColor("_Addition_Color", new Color(1, 1, 1, 0.0f));
            OnDie();
        };
    }

    protected void IncreaseStageLevelHandler(float prev, float next) {
        float ratio = next / prev;
        maxHp *= ratio;
        currentHp *= ratio;
    }

    private IEnumerator UpdateTargetPoint() {
        while(isArrive) {
            targetDirection = (TargetCharacter.position - transform.position).normalized;
            yield return new WaitForSeconds(.7f);
        }
    }
    protected void LookAt2D(float x) {
        if(x!=0)
            spriteRenderer.flipX = x>0 ? true : false;
    }

    #region IAttachmentsTakeable Implements
    public virtual void TakeAttachment(Attachment attachment) {
        attachment.transform.SetParent(this.transform);
        attachment.transform.localPosition = Vector2.zero;
        havingAttachment.Add(attachment);
        attachment.OnAttached(this);
    }
    public virtual void ReleaseAttachment(Attachment attachment) {
        havingAttachment.Remove(attachment);
        attachment.OnDetached(this);
    }
    public bool TryGetAttachment(string attachmentType, out Attachment attachment) {
        attachment = havingAttachment.Find( (item) => attachmentType == item.AttachmentType );
        return attachment==null ? false : true;
    }
    public bool TryGetAttachment<T>(out T attachment) where T : Attachment {
        attachment = (T) havingAttachment.Find( (item) => item is T );
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