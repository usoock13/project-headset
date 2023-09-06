using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, IDamageable {
    [SerializeField] protected int level = 0;
    [SerializeField] protected int MaxExp {
        get { return 50 + (int)(100 * Mathf.Pow(1.1f, level)); }
    }
    [SerializeField] protected int currentExp = 0;
    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected float currentHp = 0;

    [SerializeField] protected StateMachine stateMachine;
    
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [SerializeField] private Animator animator;
    private string currentAnimation;

    [SerializeField] protected Movement movement;
    [SerializeField] protected float moveSpeed = 5f;
    protected Vector2 moveDirection;
    protected Vector2 MoveVector {
        get { return moveDirection.normalized * moveSpeed; }
    }

    #region Attack Refer
    protected Vector2 attackingDirection;
    [SerializeField] protected bool isFixedArrow = false;
    [SerializeField] public Transform attackArrow;
    private Vector2 attackDirection;

    [SerializeField] protected Weapon basicWeapon;
    protected List<Weapon> weapons = new List<Weapon>();
    protected List<Accessory> accessories = new List<Accessory>();
    protected const int MAX_WEAPONS_COUNT = 6;
    protected const int MAX_ACCESSORIES_COUNT = 6;
    [SerializeField] protected Transform equipmentsParent;
    #endregion Attack Refer

    #region States Refer
    protected State idleState = new State("Idle");
    protected State walkState = new State("Walk");
    protected State hitState = new State("Hit");
    protected State dieState = new State("Die");
    #endregion States Refer

    protected void Awake() {
        movement = movement ?? GetComponent<Movement>();
        animator = animator ?? GetComponent<Animator>();
        spriteRenderer = spriteRenderer ?? GetComponent<SpriteRenderer>();
        InitializeStates();
    }
    protected void Start() {
        currentHp = maxHp;
        AddWeapon(basicWeapon);
    }
    protected virtual void InitializeStates() {
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
        walkState.onActive = (State previous) => {
            /* TODO : Enter Move Animation */
        };
        walkState.onStay = () => {
            MoveToward(MoveVector * Time.deltaTime);
            if(!isFixedArrow) {
                attackingDirection = moveDirection;
                RotateArrow(moveDirection);
            }
        };
        walkState.onInactive = (State previous) => {
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
            stateMachine.ChangeState(walkState);
    }
    private void MoveToward(Vector2 moveVector) {
        movement.MoveToward(moveVector);
        if(isFixedArrow) {
            if(Mathf.Abs(attackDirection.x) > 0.01f) {
                spriteRenderer.flipX = attackDirection.x<0 ? true : false;
            }
            int animationDirection = attackDirection.x<0
                                        ? (moveDirection.x<0  ?  1 : -1)
                                        : (moveDirection.x>=0 ?  1 : -1);
            animator.SetFloat("Move Animation Direction", animationDirection);
        } else {
            if(moveVector.x != 0)
                spriteRenderer.flipX = moveVector.x<0 ? true : false;
            animator.SetFloat("Move Animation Direction", 1);
        }
    }
    private void RotateArrow(Vector2 direction) {
        float rotateSpeed = 1080f;
        float rotateDir = 1f;
        float currentAngle = attackArrow.transform.rotation.eulerAngles.z;
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
        if(!isFixedArrow && active)
            attackDirection = attackArrow.transform.rotation * Vector2.up;
        isFixedArrow = active;
    }
    public void AddWeapon(Weapon weapon) {
        GameObject weaponInstance = Instantiate(weapon.gameObject, equipmentsParent);
        weapons.Add(weaponInstance.GetComponent<Weapon>());
        weaponInstance.transform.SetParent(equipmentsParent);
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
    protected void ChangeAnimation(string stateName, bool intoSelf=false) {
        if(intoSelf || stateName != currentAnimation) {
            currentAnimation = stateName;
            animator.Play(stateName);
        }
    }
    protected string CurrentAnimationName {
        get { return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name; }
    }
}