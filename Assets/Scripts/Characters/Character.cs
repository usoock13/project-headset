using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateMachine))]
public abstract class Character : MonoBehaviour, IDamageable, IAttachmentsTakeable {
    private StageManager StageManager {
        get { return GameManager.instance.StageManager; }
    }
    StageUIManager StageUIManager => GameManager.instance.StageManager._StageUIManager;
    
    private CharacterStatusUI _characterStatusUI;
    private CharacterStatusUI StatusUI => _characterStatusUI ?? GameManager.instance.StageManager._StageUIManager.CharacterStatusUI;

    [SerializeField] public Slider hpSlider;
    [SerializeField] public Slider staminaSlider;

    public CharacterInputSystem characterInputSystem;
    public GameObject GameObject => this.gameObject;

    #region Character Status
    public int level { get; protected set; } = 1;
    [SerializeField] protected int MaxExp => 50 + (int)(100 * Mathf.Pow(1.1f, level));
    protected int currentExp = 0;
    public Func<Character, float> extraExpScale;
    public float GettingExpScale { get {
        float final = 1;
        Delegate[] additions = extraExpScale.GetInvocationList();
        if(additions != null)
            for(int i=0; i<additions.Length; i++)
                final += ((Func<Character, float>) additions[i])?.Invoke(this) ?? 0;
        return final;
    }}
    public float MaxHp => maxHp;
    public float currentHp { get; protected set; }

    public float MaxStamina => maxStamina;
    public float currentStamina { get; protected set; } = 0;
    protected float defaultRecoveringStamina = 20f;
    public Func<Character, float> extraRecoveringStamina;
    protected float RecoveringStaminaPerSecond { get {
        float final = defaultRecoveringStamina;
        Delegate[] additions = extraRecoveringStamina?.GetInvocationList();
        if(additions != null)
            for(int i=0; i<additions.Length; i++)
                final += ((Func<Character, float>) additions[i])?.Invoke(this) ?? 0;
        return final;
    }}
    protected float staminaForDodge = 100f;

    public float MaxSp => maxSp;
    public float currentSP { get; protected set; } = 0;
    protected float defaultRecoverimgSP = 1f;
    public Func<Character, float> extraRecoveringSp;
    protected float RecoveringSpPerSecond { get {
        float final = defaultRecoverimgSP;
        Delegate[] additions = extraRecoveringSp?.GetInvocationList();
        if(additions != null)
            for(int i=0; i<additions.Length; i++)
                final += ((Func<Character, float>) additions[i])?.Invoke(this) ?? 0;
        return final;
    }}

    [HideInInspector] public int levelRewardCount = 0;

    private List<Attachment> havingAttachment;
    #endregion Character Status

    #region Character Information
    public Sprite DefaultSprite => defaultSprite;
    public abstract string CharacterName { get; }
    [SerializeField] private HeadmountCharacter headmountCharacter;
    [SerializeField] public HeadmountCharacter HeadmountCharacter => headmountCharacter;
    #endregion Character Information

    private ItemCollector itemCollector;
    public ItemCollector ItemCollector {
        get { return itemCollector; }
        set { 
            itemCollector = value;
            itemCollector.transform.SetParent(null);
            itemCollector.owner = this.transform;
        }
    }

    [SerializeField] protected StateMachine stateMachine;
    public State CurrentState => stateMachine.currentState;

    [SerializeField] protected Movement movement;

    private float dodgePower = 7f;
    private float dodgeDuration = .3f;
    private Coroutine dodgeCoroutine;
    [SerializeField] ParticleSystem dodgeParticle;

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected List<(SpriteRenderer hands, SpriteRenderer front, SpriteRenderer back)> hmcSpriteRenderers;
    [SerializeField] protected SpriteAnimator spriteAnimator;
    [SerializeField] protected Transform spritesParent;

    #region Status
    #region Extra Status
    public Func<Character, float> extraPower;
    public Func<Character, float> extraMoveSpeed;
    public Func<Character, float> extraAttackSpeed;
    public Func<Character, float> extraArmor;

    public Func<Monster, float, bool> attackBlocker; // Monster : Origin of attack // float : Damage amount
    #endregion Extra Status

    #region Calculated Status
    public float Power { get {
            float final = statusDefaultPower;
            Delegate[] additions = extraPower?.GetInvocationList();
            if(additions != null)
                for(int i=0; i<additions.Length; i++)
                    final += ((Func<Character, float>) additions[i])?.Invoke(this) ?? 0;
            return final;
        }
    }
    public float MoveSpeed { get {
            float final = statusDefaultMoveSpeed;
            Delegate[] additions = extraMoveSpeed?.GetInvocationList();
            if(additions != null)
                for(int i=0; i<additions.Length; i++)
                    final += ((Func<Character, float>) additions[i])?.Invoke(this) ?? 0;
            return final;
    }}
    public float AttackSpeed { get {
        float final = 1;
        Delegate[] additions = extraAttackSpeed?.GetInvocationList();
        if(additions != null)
            for(int i=0; i<additions.Length; i++)
                final += ((Func<Character, float>) additions[i])?.Invoke(this) ?? 0;
        return final;
    }}
    private const float MAX_ARMOR = 80;
    public float Armor { get {
            float final = statusDefaultArmor;
            Delegate[] additions = extraArmor?.GetInvocationList();
            if(additions != null)
                for(int i=0; i<additions.Length; i++)
                    final += ((Func<Character, float>) additions[i])?.Invoke(this) ?? 0;
            return Mathf.Min(final, MAX_ARMOR);
        }
    }
    #endregion Calculated Status
    #endregion Status

    protected Vector2 moveDirection;
    protected Vector2 MoveVector => moveDirection.normalized * MoveSpeed;

    protected Vector2 attackingDirection;
    public bool arrowIsFixed { get; protected set; } = false;
    private bool aimWithMouse = false;
    public Transform attackArrow;
    private Vector2 attackDirection;

    [SerializeField] protected Transform weaponParent;
    [SerializeField] protected Transform artifactParent;
    [SerializeField] public Transform headmountPoint;

    #region States
    public State idleState { get; protected set; } = new State("Idle");
    public State walkState { get; protected set; } = new State("Walk");
    public State dodgeState {get; protected set; } = new State("Dodge");
    public State hitState { get; protected set; } = new State("Hit");
    public State dieState { get; protected set; } = new State("Die");
    public State BasicState { get {
        if(moveDirection == Vector2.zero)
            return idleState;
        else
            return walkState;
    }}
    #endregion States

    [Header("Individual Properties")]
    [SerializeField] protected float maxHp = 100;
    [SerializeField] protected float maxStamina = 100;
    [SerializeField] protected float maxSp = 100;
    [SerializeField] protected float statusDefaultPower = 10;
    [SerializeField] protected float statusDefaultMoveSpeed = 2.5f;
    [SerializeField] protected float statusDefaultArmor = 10;
    [SerializeField] protected Weapon basicWeapon;
    [SerializeField] private Sprite defaultSprite;

    public float DefaultPower => statusDefaultPower;
    public float DefaultMoveSpeed => statusDefaultMoveSpeed;
    public float DefaultArmor => statusDefaultArmor;

    #region Character Events
    public Action<Character, Monster, float> onTakeAttack;
    public Action<Character, float> onTakeDamage;
    public Action<Character> onAttack;
    public Action<Character, Monster> onAttackMonster;
    public Action<Character, Monster> onKillMonster;
    public Action<Item> onGetItem;
    public Action<Character> onDodge;
    #endregion Character Events

    #region Unity Events
    protected void Awake() {
        movement ??= GetComponent<Movement>();
        hmcSpriteRenderers = new List<(SpriteRenderer hands, SpriteRenderer front, SpriteRenderer back)>();
        InitializeStates();
    }
    public void InitializeCharacter() {
        currentHp = maxHp;
        currentStamina = maxStamina;
        StageManager.EquipmentsManager.AddBasicWeapon(basicWeapon);
        havingAttachment = new List<Attachment>();
    }
    private void Update() {
        /* FOR TEST >> */
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.L))
            this.GetExp(100);
        #endif
        /* << FOR TEST */
        RecoverStamina();
        RecoverSkillGauge();
        RotateArrowWithMouse();
    }
    #endregion Unity Events

    protected virtual void InitializeStates() {
        stateMachine = stateMachine ?? GetComponent<StateMachine>();

        #region Initialize Move State
        walkState.onStay = () => {
            MoveToward(MoveVector * Time.deltaTime);
            if(!aimWithMouse && !arrowIsFixed) {
                attackingDirection = moveDirection;
                RotateArrow(moveDirection);
            }
        };
        #endregion Initialize Move State

        #region Initialize Dodge State
        dodgeState.onActive += (prev) => {
            onDodge?.Invoke(this);
            dodgeParticle.Play();
        };
        #endregion Initialize Dodge State

        #region Initialize Die State
        dieState.onActive += (State previous) => {
            stateMachine.isMuted = true;
            itemCollector.gameObject.SetActive(false);
        };
        #endregion Initialize Die State

        stateMachine.SetIntialState(idleState);
    }
    
    
    public void MountCharacter(HeadmountCharacter headmountCharacter) {
        HeadmountCharacter hmc = Instantiate<HeadmountCharacter>(headmountCharacter, headmountPoint);
        // hmc.headmountPoint.localScale = new Vector3(headmountPoint.localScale.x, headmountPoint.localScale.y, headmountPoint.localScale.z*2);
        hmc.HeadAbility.OnTaken(this);

        headmountPoint = hmc.headmountPoint;
        hmcSpriteRenderers.Add((hmc.HandsSprite, hmc.FrontSprite, hmc.BackSprite));
        hmc.HandsSprite.sortingOrder = hmcSpriteRenderers.Count * 2;
        hmc.FrontSprite.sortingOrder = hmcSpriteRenderers.Count;
        hmc.BackSprite.sortingOrder = -hmcSpriteRenderers.Count;
        hmc.gameObject.SetActive(true);
    }
    
    public void SetMoveDirection(Vector2 direction) {
        moveDirection = direction;
        if(!CurrentState.Compare(dodgeState))
            stateMachine.ChangeState(BasicState);
    }
    
    private void MoveToward(Vector2 moveVector) {
        movement.MoveToward(moveVector);
        if(arrowIsFixed || aimWithMouse) {
            if(Mathf.Abs(attackDirection.x) > 0.01f) {
                FlipSprites(attackDirection.x<0 ? true : false);
            }
            int animationDirection = attackDirection.x<0
                                        ? (moveDirection.x<0  ?  1 : -1)
                                        : (moveDirection.x>=0 ?  1 : -1);
            spriteAnimator.SetFloat("Move Animation Direction", animationDirection);
        } else {
            if(moveVector.x != 0)
                FlipSprites(moveVector.x<0 ? true : false);
            spriteAnimator.SetFloat("Move Animation Direction", 1);
        }
    }
    
    private void FlipSprites(bool flip) {
        Vector3 org = spritesParent.transform.localScale;
        spritesParent.localScale =  new Vector3(flip? -1 : 1, org.y, org.z);
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

    private void RotateArrowWithMouse() {
        if(aimWithMouse) {
            attackDirection = attackArrow.transform.rotation * Vector2.up;
            var point = new Vector2(Input.mousePosition.x - (Screen.width / 2), Input.mousePosition.y - (Screen.height / 2));
            attackArrow.eulerAngles = new Vector3(0, 0, (Mathf.Atan2( point.y, point.x ) - Mathf.PI/2) * Mathf.Rad2Deg);
        }
    }

    public void ToggleMouseAiming() {
        aimWithMouse = !aimWithMouse;
    }
    
    public void FixArrow(bool active) {
        if(!arrowIsFixed && active)
            attackDirection = attackArrow.transform.rotation * Vector2.up;
        arrowIsFixed = active;
    }
    
    public void DodgeToward() {
        if(moveDirection == Vector2.zero
        || currentStamina < staminaForDodge)
            return;
        if(dodgeCoroutine != null)
            StopCoroutine(dodgeCoroutine);
        ConsumeStamina(staminaForDodge);
            StartCoroutine(DodgeCoroutine());
    }
    
    private IEnumerator DodgeCoroutine() {
        stateMachine.ChangeState(dodgeState);
        float offset = 0;
        float t = 0;
        Vector2 step;
        Vector2 dir = moveDirection;
        while (offset < 1) {
            float scale = MoveSpeed / DefaultMoveSpeed; // Including move speed bonus 
            offset = Mathf.Min(offset + Time.deltaTime/dodgeDuration * scale, 1);
            t = Mathf.Sin(Mathf.PI * offset * 0.5f);
            step = Vector2.Lerp(Vector2.zero, dir.normalized, 1-t) * dodgePower * (Time.deltaTime/dodgeDuration);
            movement.MoveToward(step * scale);
            yield return null;
        }
        stateMachine.ChangeState(BasicState);
    }
    
    
    public void AddEquipment(Equipment equipment) {
        equipment.transform.SetParent(equipment is Weapon ? weaponParent : artifactParent);
        equipment.transform.localPosition = Vector2.zero;
    }
    public void RemoveEquipment(Equipment target) {
        target.transform.SetParent(StageManager.EquipmentsManager.transform);
    }
    
    private void ConsumeStamina(float amount) {
        currentStamina = Mathf.Max(currentStamina - amount, 0);
        StatusUI.UpdateStaminaSlider(currentStamina / maxStamina);
    }
    private void RecoverStamina() {
        if(currentStamina < maxStamina)
            currentStamina = Mathf.Min(currentStamina + Time.deltaTime * RecoveringStaminaPerSecond, maxStamina);
        StatusUI.UpdateStaminaSlider(currentStamina / maxStamina);
    }
    private void RecoverSkillGauge() {
        if(currentSP < maxSp)
            currentSP = Mathf.Min(currentSP + Time.deltaTime * RecoveringSpPerSecond, maxStamina);
        StatusUI.UpdateSpSlider(currentSP / maxSp);
    }
    
    public void GetExp(int amount) {
        currentExp += amount;
        if(currentExp >= MaxExp)
            LevelUp();
        StatusUI.UpdateExpSlider((float)currentExp / MaxExp);
    }
    private void LevelUp() {
        currentExp = currentExp - MaxExp;
        level ++;
        StatusUI.UpdateLevel(level);
        StageManager.OnCharacterLevelUp();
        GetExp(0); // Check multiple level up. 
    }
    
    public void OnKillMonster(Monster monster) {
        onKillMonster?.Invoke(this, monster);
    }

    public void TakeAttack(Monster origin, float amount) {
        if(CurrentState.Compare(dieState)
        || CanBlockAttack(origin, amount))
            return;
        TakeDamage(amount);
        onTakeAttack?.Invoke(this, origin, amount);
    }
    private bool CanBlockAttack(Monster origin, float amount) {
        Delegate[] blockers = attackBlocker?.GetInvocationList();
        if(blockers != null)
            for(int i=0; i<blockers.Length; i++) {
                if(((Func<Monster, float, bool>) blockers[i]).Invoke(origin, amount))
                    return true;
            }
        return false;
    }
    public void TakeDamage(float amount) {
        float finalDamage = (100 - Armor)/100 * amount;
        currentHp -= finalDamage;
        StatusUI.UpdateHpSlider(currentHp / maxHp);
        onTakeDamage?.Invoke(this, finalDamage);
        if(currentHp <= 0)
            Die();
        StageManager.PrintDamageNumber(transform.position, ((int) finalDamage).ToString(), Color.red);
        StageUIManager.ActiveHitEffectUI();
    }
    public void TakeAttackDelay(float amount) {
        throw new NotImplementedException();
        if(CurrentState.Compare(dieState))
            return;
    }
    public void TakeForce(Vector2 force, float duration=.25f) {
        throw new NotImplementedException();
        if(CurrentState.Compare(dieState))
            return;
    }
    public void TakeHeal(float amount) {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        StatusUI.UpdateHpSlider(currentHp / maxHp);
        StageManager.PrintDamageNumber(transform.position, ((int) amount).ToString(), Color.green);
    }
    private void Die() {
        stateMachine.ChangeState(dieState, false);
        StageManager.GameOver();
    }

    #region IAttachmentsTakeable Implements
    public void TakeAttachment(Attachment attachment) {
        attachment.OnAttached(this);
        havingAttachment.Add(attachment);
    }
    public void ReleaseAttachment(Attachment attachment) {
        attachment.OnDetached(this);
        havingAttachment.Remove(attachment);
    }
    public bool TryGetAttachment(string attachmentType, out Attachment attachment) {
        attachment = havingAttachment.Find((item) => {
            return attachmentType == item.AttachmentType;
        });
        return attachment==null ? false : true;
    }
    public void OnAttack() => onAttack?.Invoke(this);
    public void OnAttackMonster(Monster target) => onAttackMonster?.Invoke(this, target);
    #endregion IAttachmentsTakeable Implements
}