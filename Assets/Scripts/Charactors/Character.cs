using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour {
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private Animator animator;

    [SerializeField] private Movement movement;
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveDirection;
    private Vector2 MoveVector {
        get { return moveDirection.normalized * moveSpeed; }
    }

    #region Attack Refer
    private Vector2 attackingDirection;
    [SerializeField] private bool isFixedArrow = false;
    [SerializeField] private GameObject attackArrow;

    public UnityEvent onBasicAttack;
    #endregion

    State idleState = new State("Idle");
    State moveState = new State("Move");

    protected void Awake() {
        movement = movement ?? GetComponent<Movement>();
        animator = animator ?? GetComponent<Animator>();

        InitilizeState();
        onBasicAttack.AddListener(() => { print("Do Basic Attack!"); });
    }
    protected void InitilizeState() {
        stateMachine = stateMachine ?? GetComponent<StateMachine>();

        #region Initilize Idle State
        idleState.onActive = (State previous) => {
            /* TODO : Enter Idle Animation */
        };
        idleState.onInactive = (State next) => {
            /* TODO : Release Idle Animation */
        };
        #endregion
        #region Initilize Move State
        moveState.onActive = (State previous) => {
            /* TODO : Enter Move Animation */
        };
        moveState.onStay = () => {
            transform.Translate(MoveVector * Time.deltaTime);
            if(!isFixedArrow) {
                attackingDirection = moveDirection;
                RotateArrow(moveDirection);
            }
        };
        moveState.onInactive = (State previous) => {
            /* TODO : Release Move Animation */
        };

        stateMachine.SetIntialState(idleState);
        #endregion
    }
    public void SetMoveDirection(Vector2 direction) {
        moveDirection = direction;
        if(moveDirection == Vector2.zero)
            stateMachine.ChangeState(idleState);
        else
            stateMachine.ChangeState(moveState);
    }
    private void RotateArrow(Vector2 direction) {
        float rotateSpeed = 720f;
        float rotateDir = 1f;
        float currentAngle = attackArrow.transform.rotation.eulerAngles.z;
        // currentAngle = currentAngle <= 180 ? currentAngle : -(360 - currentAngle);
        // float directionAngle = direction.x<0 ? Vector2.Angle(Vector2.up, direction) : -Vector2.Angle(Vector2.up, direction);
        float directionAngle = direction.x>0 ? 360-Vector2.Angle(Vector2.up, direction) : Vector2.Angle(Vector2.up, direction);
        if(Mathf.Abs(currentAngle - directionAngle) > rotateSpeed * Time.deltaTime) {
            float distance = directionAngle - currentAngle;
            distance = distance>=0 ? distance : 360+distance;
            if(distance > 180) {
                rotateDir = -1f;
            }
            attackArrow.transform.Rotate(Vector3.forward * rotateSpeed * rotateDir * Time.deltaTime);
        } else {
            attackArrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg);
        }
    }
    public void FixArrow(bool active) {
        isFixedArrow = active;
    }
    protected virtual void BasciAttack() {
        onBasicAttack.Invoke();
    }
}
