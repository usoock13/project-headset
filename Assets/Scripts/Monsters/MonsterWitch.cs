using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using Utility;

public class MonsterWitch : Monster {
    public override string MonsterType => "Boss Witch";
    private Vector3 targetPointModifier = Vector3.up * 5f;

    private float maxSpeed = 6f;
    private Vector2 currentSpeed = Vector2.zero;
    private float moveSpeed = 7f;

    private const string CAST_STATES_TAG = "Cast";
    private State cast01State = new State("Cast 01", CAST_STATES_TAG); // Ink Blast
    private State cast02State = new State("Cast 02", CAST_STATES_TAG); // Eraser
    private State cast03State = new State("Cast 03", CAST_STATES_TAG); // Summon Pawns
    private State cast04State = new State("Cast 04", CAST_STATES_TAG); // Ink Storm

    private (float cost, State state) nextSkill;

    private readonly float skillCost02 = 4f;
    private readonly float skillCost01 = 3f;
    private readonly float skillCost03 = 5f;

    private float maxSkillGauge = 10f;
    private float skillGauge = 0f;

    private const string ANIMATION_CHASE = "Witch Chase";
    private const string ANIMATION_DIE = "Witch Die";
    private const string ANIMATION_CAST_01 = "Witch Cast 01";
    private const string ANIMATION_CAST_02 = "Witch Cast 02";

    [SerializeField] private GameObject effectInkBlast;
    [SerializeField] private GameObject effectEraser;
    [SerializeField] private GameObject effectPawn;
    [SerializeField] private EffectInkStorm effectInkStorm;
    [SerializeField] private GameObject attachmentStun;

    private float shieldRatio = 0.50f;
    private float shieldAmount = 0f;
    [SerializeField] private SpriteRenderer shieldRenderer;
    private bool HasShield => shieldAmount > 0;

    public ObjectPooler InkPooler { get; private set; }
    public ObjectPooler EraserPooler { get; private set; }
    public ObjectPooler PawnsPooler { get; private set; }
    public ObjectPooler StunPooler { get; private set; }

    #region Skill Status
    private float blastDamage = 28f;
    private float eraserDPS = 98f;
    private float pawnDamage = 40f;
    private float stormDamage = 50f;
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
        PawnsPooler = new ObjectPooler(
            poolingObject: effectPawn,
            parent: this.transform
        );
        StunPooler = new ObjectPooler(
            poolingObject: attachmentStun,
            parent: this.transform
        );
    }

    private void Update() {
        skillGauge += Time.deltaTime;
        skillGauge = skillGauge > maxSkillGauge ? maxSkillGauge : skillGauge;
    }

    protected override void InitializeStates() {
        base.InitializeStates();
        
        #region Chase
        chaseState.onStay += () => {
            spriteAnimator.ChangeAnimation(ANIMATION_CHASE);
        };
        chaseState.onStay += () => {
            MoveToTarget();
        };
        #endregion Chase

        #region Ink Blast
        cast01State.onActive += (prev) => Cast01();
        cast01State.onStay += () => FlyWithCurrentVelocity();
        #endregion Ink Blast
        
        #region Eraser
        cast02State.onActive += (prev) => {
            Cast02();
            spriteAnimator.ChangeAnimation(ANIMATION_CAST_02);
        };
        cast02State.onStay += () => MoveToTarget();
        #endregion Eraser

        #region Summon Pawn
        cast03State.onActive += (prev) => Cast03();
        cast03State.onStay += () => MoveToTarget();
        #endregion Summon Pawn

        #region Ink Storm
        cast04State.onActive += (prev) => Cast04();
        cast04State.onInactive += (prev) => InactiveInkStorm();
        #endregion Ink Storm

        SelecteNextSkill(); // Initial Selecting
        StartCoroutine(CastCoroutine());

        stateMachine.SetIntialState(chaseState);
        stateMachine.ChangeState(chaseState);
    }

    private IEnumerator CastCoroutine() {
        yield return new WaitForCondition(() => skillGauge >= nextSkill.cost);
        // Wait for skill gauge reach to next skill's cost.

        if(!stateMachine.Compare(CAST_STATES_TAG)
        && isArrive) {
            skillGauge -= nextSkill.cost;
            stateMachine.ChangeState(nextSkill.state);

            SelecteNextSkill();
            StartCoroutine(CastCoroutine());
        }
    }

    private void SelecteNextSkill() {
        /*  */
        nextSkill = (0, cast04State);
        return;
        /*  */
        if(isArrive) {
            int choise = /* UnityEngine.Random.Range(0, 8) */7;
            nextSkill  = choise switch {
                <= 3 => (skillCost01, cast01State), // 0, 1, 2, 3
                <= 6 => (skillCost02, cast02State), // 4, 5, 6
                <= 7 => (skillCost03, cast03State), // 7
                _    => (skillCost01, cast01State), // Make be never happen...
            };
        }
    }

    #region Cast Ink Blast
    private void Cast01() {
        skillGauge -= skillCost01;
        StartCoroutine(CastInkBlastsCoroutine());
    }
    private IEnumerator CastInkBlastsCoroutine() {
        Vector3 start = transform.position;
        Vector3 point = TargetCharacter.transform.position;
        for(int i=1; i<=5; i++) {
            CastInkBlast(transform.position, point - start + new Vector3(i-2, 3-i, 0));
            yield return new WaitForSeconds(0.2f);
        }
        stateMachine.ChangeState(chaseState);
    }
    private void CastInkBlast(Vector2 center, Vector2 dir) {
        var eft = InkPooler.OutPool();

        eft.transform.position = center;
        eft.transform.LookAtWithUp(new Vector2(transform.position.x, transform.position.y) + dir);
        eft.GetComponent<EffectInkBlast>()?.Active(blastDamage, InkPooler);
    }
    #endregion Cast Ink Blast

    #region Cast Eraser
    private void Cast02() {
        StartCoroutine(EraserCoroutine());
    }
    private IEnumerator EraserCoroutine() {
        spriteAnimator.ChangeAnimation(ANIMATION_CAST_02);
        SummonEraser();

        yield return new WaitForSeconds(1.25f);

        stateMachine.ChangeState(chaseState);
    }
    private void SummonEraser() {
        var eft = EraserPooler.OutPool(TargetCharacter.transform.position, Quaternion.identity);
        eft.GetComponent<EffectEraser>()?.Active(eraserDPS, EraserPooler, TargetCharacter.transform);
    }
    #endregion Cast Eraser

    #region Cast Summoning Pawns
    private void Cast03() {
        StartCoroutine(SummonPawns());
    }

    private IEnumerator SummonPawns() {
        Vector3 moveDir = TargetCharacter.MoveDirection == Vector2.zero
                          ? Quaternion.AngleAxis(UnityEngine.Random.Range(0, 8) * 90f, Vector3.forward) * Vector2.up
                          : TargetCharacter.MoveDirection;
        Vector2 point = TargetCharacter.transform.position + (moveDir * 3f);
        
        PawnsPooler.OutPool(point + new Vector2(moveDir.y, -moveDir.x) *  0.5f, Quaternion.identity).GetComponent<EffectPawn>().Active(pawnDamage, PawnsPooler);
        PawnsPooler.OutPool(point + new Vector2(moveDir.y, -moveDir.x) * -0.5f, Quaternion.identity).GetComponent<EffectPawn>().Active(pawnDamage, PawnsPooler);
        yield return new WaitForSeconds(0.25f);
        PawnsPooler.OutPool(point + new Vector2(moveDir.y, -moveDir.x) *  1.5f, Quaternion.identity).GetComponent<EffectPawn>().Active(pawnDamage, PawnsPooler);
        PawnsPooler.OutPool(point + new Vector2(moveDir.y, -moveDir.x) * -1.5f, Quaternion.identity).GetComponent<EffectPawn>().Active(pawnDamage, PawnsPooler);
        yield return new WaitForSeconds(0.25f);
        PawnsPooler.OutPool(point + new Vector2(moveDir.y, -moveDir.x) *  2.5f, Quaternion.identity).GetComponent<EffectPawn>().Active(pawnDamage, PawnsPooler);
        PawnsPooler.OutPool(point + new Vector2(moveDir.y, -moveDir.x) * -2.5f, Quaternion.identity).GetComponent<EffectPawn>().Active(pawnDamage, PawnsPooler);
    }
    #endregion Cast Summoning Pawns

    #region Cast Ink Storm
    private void Cast04() {
        IncreaseShield(maxHp * shieldRatio);
        effectInkStorm.Active(stormDamage);
        StartCoroutine(SubCastCoroutine01());
        StartCoroutine(SubCastCoroutine02());
    }
    private void InactiveInkStorm() {
        effectInkStorm.Inactive();
    }
    private IEnumerator SubCastCoroutine01() {
        yield return new WaitForSeconds(2.5f);
        while(stateMachine.Compare(cast04State)) {
            CastInkBlast(transform.position, new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)));
            yield return new WaitForSeconds(0.1f);
            CastInkBlast(transform.position, new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)));
            yield return new WaitForSeconds(0.1f);
            CastInkBlast(transform.position, new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)));
            yield return new WaitForSeconds(1.5f);
        }
    }
    private IEnumerator SubCastCoroutine02() {
        yield return new WaitForSeconds(2.5f);
        while(stateMachine.Compare(cast04State)) {
            SummonEraser();
            yield return new WaitForSeconds(4f);
        }
    }
    #endregion Cast Ink Storm

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

        FlyWithCurrentVelocity();
    }
    private void FlyWithCurrentVelocity() {
        movement.Translate(currentSpeed * Time.deltaTime);
        LookAt2D(transform.position.x - TargetCharacter.transform.position.x);
    }

    private void IncreaseShield(float amount) {
        if(shieldAmount > 0)
            shieldAmount += amount;
        else
            shieldAmount = amount;
        shieldRenderer.enabled = true;
    }

    private float DeductShiled(float damage) {
        if(shieldAmount <= 0)
            return damage;

        shieldAmount -= damage;
        if(shieldAmount >= 0) {
            _StageManager.PrintDamageNumber(transform.position, ((int) damage).ToString(), new Color(0.02f, 0.02f, 0.02f, 1f));
            return 0;
        } else {
            _StageManager.PrintDamageNumber(transform.position, ((int) damage + shieldAmount).ToString(), new Color(0.02f, 0.02f, 0.02f, 1f));
            shieldRenderer.enabled = false;
            OnShieldBreaks();
            return -shieldAmount;
        }
    }

    private void OnShieldBreaks() {
        stateMachine.ChangeState(chaseState);
    }

    public override void TakeDamage(float amount) {
        amount = DeductShiled(amount);
        if(amount <= 0)
            return;
        base.TakeDamage(amount);
    }

    public override void TakeAttackDelay(float second) {
        if(HasShield)
            return;
        StartCoroutine(TakeBitDelayCoroutine());
    }

    public override void TakeForce(Vector2 force, float duration) {
        if(HasShield)
            return;
        base.TakeForce(force * 0.1f, duration);
    }
}