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
    [SerializeField] public Transform attackDirection;

    [SerializeField] private Weapon basicWeapon;
    private List<Weapon> weapons = new List<Weapon>();
    private List<Accessory> accessories = new List<Accessory>();
    private const int MAX_WEAPONS_COUNT = 6;
    private const int MAX_ACCESSORIES_COUNT = 6;
    [SerializeField] private Transform equipmentsParent;
    #endregion Attack Refer

    State idleState = new State("Idle");
    State moveState = new State("Move");

    protected void Awake() {
        movement = movement ?? GetComponent<Movement>();
        animator = animator ?? GetComponent<Animator>();

        InitilizeState();
    }
    protected void Start() {
        AddWeapon(basicWeapon);
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
        float currentAngle = attackDirection.transform.rotation.eulerAngles.z;
        float directionAngle = direction.x>0 ? 360-Vector2.Angle(Vector2.up, direction) : Vector2.Angle(Vector2.up, direction);
        if(Mathf.Abs(currentAngle - directionAngle) > rotateSpeed * Time.deltaTime) {
            float distance = directionAngle - currentAngle;
            distance = distance>=0 ? distance : 360+distance;
            if(distance > 180) {
                rotateDir = -1f;
            }
            attackDirection.transform.Rotate(Vector3.forward * rotateSpeed * rotateDir * Time.deltaTime);
        } else {
            attackDirection.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg);
        }
    }
    public void FixArrow(bool active) {
        isFixedArrow = active;
    }
    public void AddWeapon(Weapon weapon) {
        Instantiate(weapon.gameObject, equipmentsParent);
        weapons.Add(weapon);
    }
}