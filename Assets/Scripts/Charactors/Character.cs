using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, IDamageable {
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float currentHp = 0;

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

    #region States Refer
    State idleState = new State("Idle");
    State moveState = new State("Move");
    State hitState = new State("Hit");
    State dieState = new State("Die");
    #endregion States Refer

    #region Animation Clips
    const string ANIMATION_IDLE = "Warrior Idle";
    const string ANIMATION_WALK = "Warrior Walk";
    #endregion Animation Clips

    protected void Awake() {
        movement = movement ?? GetComponent<Movement>();
        animator = animator ?? GetComponent<Animator>();
        InitilizeState();
    }
    protected void Start() {
        currentHp = maxHp;
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
        #endregion Initilize Idle State
        #region Initilize Move State
        moveState.onActive = (State previous) => {
            /* TODO : Enter Move Animation */
        };
        moveState.onStay = () => {
            movement.MoveToward(MoveVector * Time.deltaTime);
            if(!isFixedArrow) {
                attackingDirection = moveDirection;
                RotateArrow(moveDirection);
            }
        };
        moveState.onInactive = (State previous) => {
            /* TODO : Release Move Animation */
        };
        #endregion Initilize Move State
        #region Initilize Die State
        dieState.onActive += (State previous) => {
            stateMachine.isMuted = true;
            
        };
        #endregion Initilize Die State

        stateMachine.SetIntialState(idleState);
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
    public void TakeDamage(float amount) {
        currentHp -= amount;
        if(currentHp <= 0) {
            
        }
    }
    public void TakeAttackDelay(float amount) {
        throw new System.NotImplementedException();
    }
    public void TakeForce(Vector2 force) {
        throw new System.NotImplementedException();
    }
    private void Die() {
        stateMachine.ChangeState(dieState, false);
        GameManager.instance.StageManager.GameOver();
    }
}