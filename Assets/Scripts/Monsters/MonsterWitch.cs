using System;
using System.Collections;
using UnityEngine;

public class MonsterWitch : Monster {
    public override string MonsterType => "Boss Witch";
    private Transform Target => GameManager.instance.Character.transform;

    [SerializeField] Transform __debugger;

    private const string CAST_STATES_TAG = "Cast";
    private State cast01State = new State("Cast 01", CAST_STATES_TAG);
    private State cast02State = new State("Cast 02", CAST_STATES_TAG);
    private State cast03State = new State("Cast 03", CAST_STATES_TAG);

    private float attackCost;

    private Coroutine moveCoroutine;

    protected override void InitializeStates() {
        base.InitializeStates();

        chaseState.onActive += (State prev) => {
            moveCoroutine = StartCoroutine(MoveToTarget());
        };
        chaseState.onStay += () => {
            LookAt2D(Target.position.x - transform.position.x);
        };
        chaseState.onInactive += (State next) => {
            if(moveCoroutine != null)
                StopCoroutine(moveCoroutine);
        };

        stateMachine.SetIntialState(chaseState);
        stateMachine.ChangeState(chaseState);
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

    private IEnumerator MoveToTarget() {
        Vector2 start = transform.position;
        Vector2 dest = new Vector2(
                        Target.position.x + UnityEngine.Random.Range(-5f, 5f),
                        Target.position.y + UnityEngine.Random.Range(-5f, 5f));
        float offset = 0;

        /* debug >> */
        if(__debugger == null)
            __debugger =  GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        __debugger.position = dest;
        /* << debug */
        
        float cx = transform.position.x - Target.position.x;
        float cy = transform.position.y - Target.position.y;
        float dx = dest.x - Target.position.x;
        float dy = dest.y - Target.position.y;
        float angle = cx>0 && dx>0 || cx<0 && dx<0 || cy>0 && dy>0 || cy<0 && dy<0 ? -90 : 90;

        Vector2 path = Quaternion.AngleAxis(angle, Vector3.forward) * (dest - start) * 0.5f;

        while(offset < 1) {
            offset += Time.deltaTime;
            
            Vector2 end = Vector2.Lerp(path, dest, offset);
            
            transform.position = Vector2.Lerp(start, end, offset);

            yield return null;
        }
        stateMachine.ChangeState(chaseState);
    }
    
    private void CastSkill() {
        /* 
            DO SOMETHING.
        */
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