using System;
using System.Collections;
using UnityEngine;
using Utility;

public class MonsterWitch : Monster {
    public override string MonsterType => "Boss Witch";
    private Vector3 targetPointModifier = Vector3.up * 5f;

    private float maxSpeed = 6f;
    private Vector2 currentSpeed = Vector2.zero;
    private float moveSpeed = 7f;

    private const string CAST_STATES_TAG = "Cast";
    private State cast01State = new State("Cast 01", CAST_STATES_TAG);
    private State cast02State = new State("Cast 02", CAST_STATES_TAG);
    private State cast03State = new State("Cast 03", CAST_STATES_TAG);

    private float maxSkillGauge = 100f;
    private float skillGauge = 0f;

    [SerializeField] private GameObject effectInkBlast;
    [SerializeField] private GameObject effectEraser;

    public ObjectPooler InkPooler { get; private set; }
    public ObjectPooler EraserPooler { get; private set; }

    #region Skill Status
    private float inkDamage = 28f;
    private float eraserDPS = 98f;
    #endregion Skill Status

    protected override void Awake() {
        base.Awake();

        InkPooler = new ObjectPooler(
            poolingObject: effectInkBlast,
            parent: this.transform
        );
        EraserPooler = new ObjectPooler(
            poolingObject: effectEraser,
            parent: this.transform
        );
    }

    protected override void InitializeStates() {
        base.InitializeStates();
        
        chaseState.onStay += () => {
            Attack();
            MoveToTarget();
        };

        stateMachine.SetIntialState(chaseState);
        stateMachine.ChangeState(chaseState);
    }

    private void Attack() {
        /* __ test code >> */
        skillGauge += Time.deltaTime;
        if(skillGauge >= 2.5f) {
            skillGauge -=  2.5f;
            // StartCoroutine(CastInk());
            CastEraser();
        }
        /* << __ test code */
    }
    private IEnumerator CastInk() {
        Vector3 start = transform.position;
        for(int i=1; i<=5; i++) {
            InkBlast(transform.position, TargetCharacter.transform.position - start + new Vector3(i-2, 3-i, 0));
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void CastEraser() {
        var eft = EraserPooler.OutPool(TargetCharacter.transform.position, Quaternion.identity);

        eft.GetComponent<EffectEraser>()?.Active(eraserDPS, EraserPooler, TargetCharacter.transform);
    }

    private void InkBlast(Vector2 center, Vector2 dir) {
        var eft = InkPooler.OutPool();

        eft.transform.position = center;
        eft.transform.LookAtWithUp(new Vector2(transform.position.x, transform.position.y) + dir);
        eft.GetComponent<EffectInkBlast>()?.Active(inkDamage, InkPooler);
    }

    public override void OnSpawn() {
        isArrive = true;
        maxHp = defaultHp;
        currentHp = maxHp;
    }
    protected override void OnDie() {
        isArrive = false;
        rigidbody2D.simulated = false;
    }

    private void MoveToTarget() {
        #region Rotate 'Target Point Modifier'
        targetPointModifier = Quaternion.AngleAxis(Time.deltaTime * 120f, Vector3.forward) * targetPointModifier;
        #endregion Rotate 'Target Point Modifier'

        #region Accelerate the Witch
        Vector3 targetPoint = TargetCharacter.transform.position + targetPointModifier;
        Vector2 dir = (targetPoint - transform.position).normalized;
        currentSpeed += Time.deltaTime * moveSpeed * dir;
        currentSpeed = currentSpeed.magnitude > maxSpeed ? currentSpeed.normalized * maxSpeed : currentSpeed;
        #endregion Accelerate the Witch

        movement.Translate(currentSpeed * Time.deltaTime);
        LookAt2D(transform.position.x - TargetCharacter.transform.position.x);
    }

    public override void TakeDamage(float amount) {
        base.TakeDamage(amount);
    }

    public override void TakeAttackDelay(float second) {
        StartCoroutine(TakeBitDelayCoroutine());
    }

    public override void TakeForce(Vector2 force, float duration) {
        base.TakeForce(force * 0.1f, duration);
    }
}