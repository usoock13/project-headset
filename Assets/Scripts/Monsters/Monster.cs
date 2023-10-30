using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject GameObject => this.gameObject;

    [SerializeField] protected Movement movement;
    [SerializeField] protected StateMachine stateMachine;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteAnimator spriteAnimator;
    [SerializeField] new protected Collider2D collider2D;
    [SerializeField] new protected Rigidbody2D rigidbody2D;

    [SerializeField] protected float defaultMoveSpeed = 5f;
    public Func<Monster, float> moveSpeedScales;
    public float MoveSpeed { get {
        float final = defaultMoveSpeed;
        float max = 1f;
        float min = 1f;
        Delegate[] scales = moveSpeedScales?.GetInvocationList();
        if(scales != null)
            for(int i=0; i<scales.Length; i++) {
                Func<Monster, float> f = (Func<Monster, float>) scales[i];
                if(f(this) < min)
                    min = f(this);
                if(f(this) > max)
                    max = f(this);
            }
        final *= min * max;
        return final>0 ? final : 0;
    }}
    protected Vector2 targetDirection;
    protected Vector2 MoveVector {
        get { return targetDirection * MoveSpeed; }
    }
    public abstract string MonsterType { get; }

    [SerializeField] protected State chaseState = new State("Chase");
    [SerializeField] protected State hitState = new State("Hit");
    [SerializeField] protected State dieState = new State("Die");

    public bool isArrive { get; protected set;} = true;
    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected float currentHp = 0;
    private List<Attachment> havingAttachment = new List<Attachment>();
    public List<Attachment> H => havingAttachment;

    public UnityAction<Monster> onDie;

    [SerializeField] protected int givingExp = 10;

    private Coroutine takeHittingDelayCoroutine;
    private Coroutine updateTargetDirectionCoroutine;

    [SerializeField] protected ObjectPooler ownerPooler = null;

    #region IDamageable Define
    public virtual void TakeDamage(float amount) {
        currentHp -= amount;
        if(currentHp <= 0)
            stateMachine.ChangeState(dieState);
        PrintDamageNumber((int) amount);
    }
    protected virtual void PrintDamageNumber(int amount) {
        try {
            Vector2 point = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-.3f, .3f), 0);
            GameManager.instance.StageManager.PrintDamageNumber(point, amount.ToString());
        } catch(Exception e) {
            Debug.LogError(e.StackTrace);
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
        if(takeHittingDelayCoroutine != null)
            StopCoroutine(takeHittingDelayCoroutine);
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
        DropExp();
        onDie?.Invoke(this);
        ClearAttachments();
        StopCoroutine(updateTargetDirectionCoroutine);
    }
    protected void DropExp() {
        GameManager.instance.StageManager.CreateExp(transform.position, givingExp);
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
}