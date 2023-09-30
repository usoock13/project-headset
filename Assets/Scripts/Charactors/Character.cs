using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(SpriteAnimator))]
[RequireComponent(typeof(StateMachine))]
public class Character : MonoBehaviour, IDamageable {
    private StageManager StageManager {
        get { return GameManager.instance.StageManager; }
    }
    private CharacterStatusUI _characterStatusUI;
    private CharacterStatusUI StatusUI => _characterStatusUI ?? GameManager.instance.StageManager.StageUIManager.CharacterStatusUI;

    public int level { get; protected set; } = 1;
    [SerializeField] protected int MaxExp => 50 + (int)(100 * Mathf.Pow(1.1f, level));
    [SerializeField] protected int currentExp = 0;
    public int levelRewardCount = 0;

    [SerializeField] protected float maxHp = 100;
    [SerializeField] public float MaxHp { get; }
    [SerializeField] public float currentHp { get; protected set; }

    [SerializeField] protected StateMachine stateMachine;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteAnimator spriteAnimator;
    [SerializeField] protected Movement movement;

    #region Status
    [SerializeField] protected float statusDefaultPower = 10;
    [SerializeField] protected float statusDefaultMoveSpeed = 5;
    [SerializeField] protected float statusDefaultArmor = 10;
    // |
    #region Additional Status
    public Func<float> additionalPower;
    public Func<float> additionalMoveSpeed;
    public Func<float> additionalArmor;
    #endregion Additional Status
    // |
    [SerializeField] public float Power {
        get {
            float final = statusDefaultPower;
            Delegate[] additions = additionalPower?.GetInvocationList();
            if(additions != null)
                for(int i=0; i<additions.Length; i++)
                    final += ((Func<float>) additions[i])?.Invoke() ?? 0;
            return final;
        }
    }
    [SerializeField] protected float MoveSpeed {
        get {
            float final = statusDefaultMoveSpeed;
            Delegate[] additions = additionalMoveSpeed?.GetInvocationList();
            if(additions != null)
                for(int i=0; i<additions.Length; i++)
                    final += ((Func<float>) additions[i])?.Invoke() ?? 0;
            return final;
        }
    }
    [SerializeField] protected float Armor {
        get {
            float final = statusDefaultArmor;
            Delegate[] additions = additionalArmor?.GetInvocationList();
            if(additions != null)
                for(int i=0; i<additions.Length; i++)
                    final += ((Func<float>) additions[i])?.Invoke() ?? 0;
            return final;
        }
    }
    #endregion Status

    protected Vector2 moveDirection;
    protected Vector2 MoveVector {
        get { return moveDirection.normalized * MoveSpeed; }
    }

    protected Vector2 attackingDirection;
    public bool arrowIsFixed { get; protected set; } = false;
    public Transform attackArrow;
    private Vector2 attackDirection;

    [SerializeField] protected Weapon basicWeapon;
    [SerializeField] protected Transform weaponParent;
    [SerializeField] protected Transform artifactParent;

    #region States Refer
    protected State idleState = new State("Idle");
    protected State walkState = new State("Walk");
    protected State hitState = new State("Hit");
    protected State dieState = new State("Die");
    #endregion States Refer

    protected void Awake() {
        movement ??= GetComponent<Movement>();
        spriteAnimator ??= GetComponent<SpriteAnimator>();
        spriteRenderer = spriteRenderer ?? GetComponent<SpriteRenderer>();
        InitializeStates();
    }
    protected void Start() {
        currentHp = maxHp;
        StageManager.EquipmentsManager.AddBasicWeapon(basicWeapon);
    }
    protected virtual void InitializeStates() {
        stateMachine = stateMachine ?? GetComponent<StateMachine>();

        #region Initilize Move State
        walkState.onStay = () => {
            MoveToward(MoveVector * Time.deltaTime);
            if(!arrowIsFixed) {
                attackingDirection = moveDirection;
                RotateArrow(moveDirection);
            }
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
        if(arrowIsFixed) {
            if(Mathf.Abs(attackDirection.x) > 0.01f) {
                spriteRenderer.flipX = attackDirection.x<0 ? true : false;
            }
            int animationDirection = attackDirection.x<0
                                        ? (moveDirection.x<0  ?  1 : -1)
                                        : (moveDirection.x>=0 ?  1 : -1);
            spriteAnimator.SetFloat("Move Animation Direction", animationDirection);
        } else {
            if(moveVector.x != 0)
                spriteRenderer.flipX = moveVector.x<0 ? true : false;
            spriteAnimator.SetFloat("Move Animation Direction", 1);
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
        if(!arrowIsFixed && active)
            attackDirection = attackArrow.transform.rotation * Vector2.up;
        arrowIsFixed = active;
    }
    public void AddWeapon(Weapon weapon) {
        weapon.transform.SetParent(weaponParent);
        /* 
            *** TODO : Update UI that show Chracter information. ***
        */
    }
    public void AddArtifact(Artifact artifact) {
        artifact.transform.SetParent(artifactParent);
        /* 
            *** TODO : Update UI that show Chracter information. ***
        */
    }

    /* __temporary >> */
    private void Update() {
        if(Input.GetKeyDown(KeyCode.L))
            this.GetExp(100);
    }
    /* << __temporary */
    
    public void GetExp(int amount) {
        currentExp += amount;
        if(currentExp >= MaxExp)
            LevelUp();
        StatusUI.UpdateExpSlider((float)currentExp / MaxExp);
    }
    private void LevelUp() {
        currentExp = currentExp - MaxExp;
        level ++;
        /* 
            *** TODO : Update UI that show Character Level and Exp point. ***
        */
        StageManager.OnCharacterLevelUp();
        GetExp(0); // Check multiple level up. 
    }
    public void TakeDamage(float amount) {
        currentHp -= amount;
        if(currentHp <= 0) {
            Die();
        }
    }
    public void TakeHittingDelay(float amount) {
        throw new System.NotImplementedException();
    }
    public void TakeForce(Vector2 force, float duration=.25f) {
        throw new System.NotImplementedException();
    }
    private void Die() {
        stateMachine.ChangeState(dieState, false);
        StageManager.GameOver();
    }

    [System.Serializable]
    public class HeadAbility {
        public Sprite icon;
        public string name;
        public string description;
        public Action<Character> onJoinParty;
    }
}