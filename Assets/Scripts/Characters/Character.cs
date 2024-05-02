using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Utility;

[RequireComponent(typeof(StateMachine))]
public abstract class Character : MonoBehaviour, IDamageable, IAttachmentsTakeable {
    private StageManager StageManager {
        get { return GameManager.instance.StageManager; }
    }
    StageUIManager StageUIManager => GameManager.instance.StageManager.StageUIManager;
    
    private CharacterStatusUI _characterStatusUI;
    private CharacterStatusUI StatusUI => _characterStatusUI ?? GameManager.instance.StageManager.StageUIManager.CharacterStatusUI;

    [SerializeField] public Slider hpSlider;
    [SerializeField] public Slider staminaSlider;

    public CharacterInputSystem characterInputSystem;
    public GameObject GameObject => this.gameObject;

    #region Character Status
    public int level { get; protected set; } = 1;
    [SerializeField] protected int MaxExp => 50*level + 10*level*level;
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

    public bool canMove = true;

    public float MaxSp => maxSp;
    public float CurrentSP { get; protected set; } = 0;
    protected float defaultRecoveringSP = 1f;
    public Func<Character, float> extraRecoveringSp;
    protected float RecoveringSpPerSecond { get {
        float final = defaultRecoveringSP;
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

    [SerializeField] private Skill activeSkill;
    public (Sprite icon, string name, string description) SkillInfo => 
        (activeSkill.Icon, activeSkill.Name, activeSkill.Description);

    [SerializeField] private HeadmountCharacter headmountCharacter;
    public HeadmountCharacter HeadmountCharacter => headmountCharacter;
    #endregion Character Information

    private ItemCollector itemCollector;
    public ItemCollector ItemCollector {
        get { return itemCollector; }
        set { itemCollector = value; }
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

    public Func<GameObject, float, bool> attackBlocker; // GameObject : Origin of attack // float : Damage amount
    #endregion Extra Status

    #region Calculated Status
    public float Power { get {
            float final = statusDefaultPower;
            final += level * increasingPower;
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
    public Vector2 MoveDirection => moveDirection;
    protected Vector2 MoveVector => moveDirection.normalized * MoveSpeed;

    private bool isArrowLocked = false;
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

    [SerializeField] protected float increasingPower = 0.2f;
    [SerializeField] protected float increasingHP = 2f;
    
    [SerializeField] protected Weapon basicWeaponPrefab;
    [SerializeField] protected Weapon awakenWeaponPrefab;
    public (Sprite icon, string name) BasicWeaponInfo => (basicWeaponPrefab.Icon, basicWeaponPrefab.Name);
    public (Sprite icon, string name) AwakenWeaponInfo => (awakenWeaponPrefab.Icon, awakenWeaponPrefab.Name);

    [SerializeField] private Sprite defaultSprite;

    public float DefaultPower => statusDefaultPower;
    public float DefaultMoveSpeed => statusDefaultMoveSpeed;
    public float DefaultArmor => statusDefaultArmor;
    public float DefaultRecoveringSP => defaultRecoveringSP;
    public float DefaultRecoveringStamina => defaultRecoveringStamina;

    #region Character Events
    public Action<Character, GameObject, float> onTakeAttack;
    public Action<Character, float> onTakeDamage;
    public Action<Character> onAttack;
    public Action<Character, Monster> onAttackMonster;
    public Action<Character, Monster> onKillMonster;
    public Action<Item> onGetItem;
    public Action<Character> onDodge;
    #endregion Character Events

    [SerializeField] private Light2D handLampLight;

    [SerializeField] private AudioClip castingSkillSound;

    #region Unity Events
    protected void Awake() {
        movement ??= GetComponent<Movement>();
        hmcSpriteRenderers = new List<(SpriteRenderer hands, SpriteRenderer front, SpriteRenderer back)>();
        InitializeStates();
    }
    public void InitializeCharacter() {
        currentHp = maxHp;
        currentStamina = maxStamina;
        StageManager.EquipmentsManager.AddBasicWeapon(basicWeaponPrefab);
        havingAttachment = new List<Attachment>();
    }
    private void Update() {
        /* FOR TEST >> */
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.L))
            this.GetExp((int) (MaxExp * 0.5f));
        if(Input.GetKeyDown(KeyCode.O))
            RecoverSkillGauge(100);
        if(Input.GetKeyDown(KeyCode.P))
            StageManager.CameraDirector.ShakeCameraCurved(2, 2);
        #endif
        /* << FOR TEST */
        RecoverStamina(Time.deltaTime * RecoveringStaminaPerSecond);
        RecoverSkillGauge(Time.deltaTime * RecoveringSpPerSecond);
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
                RotateArrowKeyboad(moveDirection);
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
        if(canMove
        && !CurrentState.Compare(dodgeState))
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
    
    private void RotateArrowKeyboad(Vector2 direction) {
        if(isArrowLocked)
            return;

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
        if(aimWithMouse && !isArrowLocked) {
            Vector2 screenPos = (Camera.main.WorldToScreenPoint(attackArrow.transform.position));
            attackDirection = attackArrow.transform.rotation * Vector2.up;
            var point = new Vector2(Input.mousePosition.x - screenPos.x, Input.mousePosition.y - screenPos.y);
            attackArrow.eulerAngles = new Vector3(0, 0, (Mathf.Atan2( point.y, point.x ) - Mathf.PI/2) * Mathf.Rad2Deg);
        }
    }

    public void RotateArrow(Vector2 direction) {
        Vector2 position2d = attackArrow.transform.position;
        attackArrow.LookAtWithUp(position2d + direction);
    }

    public void LockAttackArrow(bool locked) {
        isArrowLocked = locked;
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
        || stateMachine.currentState.Compare(dieState)
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
            float scale = MoveSpeed / DefaultMoveSpeed; // Including movement speed bonus 
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
    
    public void GetExp(int amount) {
        currentExp += amount;
        if(currentExp >= MaxExp)
            LevelUp();
        StatusUI.UpdateExpSlider((float)currentExp / MaxExp);
    }
    private void LevelUp() {
        currentExp = currentExp - MaxExp;
        level ++;
        maxHp += increasingHP;
        currentHp += increasingHP;

        StatusUI.UpdateHpSlider(currentHp / maxHp);
        StatusUI.UpdateLevel(level);
        StageManager.OnCharacterLevelUp();
        GetExp(0); // Check multiple level up. 
    }
    
    public void OnKillMonster(Monster monster) {
        onKillMonster?.Invoke(this, monster);
    }
    
    private bool CanBlockAttack(GameObject origin, float amount) {
        Delegate[] blockers = attackBlocker?.GetInvocationList();
        if(blockers != null)
            for(int i=0; i<blockers.Length; i++) {
                if(((Func<GameObject, float, bool>) blockers[i]).Invoke(origin, amount))
                    return true;
            }
        return false;
    }   
    public void TakeDamage(float amount, GameObject origin=null) {
        if(CurrentState.Compare(dieState)
        || CanBlockAttack(origin, amount))
            return;

        float finalDamage = (100 - Armor)/100 * amount;
        currentHp -= finalDamage;
        StatusUI.UpdateHpSlider(currentHp / maxHp);
        onTakeDamage?.Invoke(this, finalDamage);
        if(currentHp <= 0)
            Die();
        StageManager.PrintDamageNumber(transform.position, ((int) finalDamage).ToString(), Color.red);
        StageUIManager.ActiveHitEffectUI();
        onTakeAttack?.Invoke(this, origin, amount);
    }
    public void ConsumeHP(float amount) {
        currentHp -= amount;
        if(currentHp <= 0)
            currentHp = 1;
        StatusUI.UpdateHpSlider(currentHp / maxHp);
    }
    public void TakeStagger(float second, GameObject origin=null) {
        throw new NotImplementedException();
        if(CurrentState.Compare(dieState))
            return;
    }
    public void TakeForce(Vector2 force, float duration=.25f, GameObject origin=null) {
        throw new NotImplementedException();
        if(CurrentState.Compare(dieState))
            return;
    }
    public void TakeHeal(float amount) {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        StatusUI.UpdateHpSlider(currentHp / maxHp);
        StageManager.PrintDamageNumber(transform.position, ((int) amount).ToString(), Color.green);
    }
    public void IncreaseMaxHp(int amount) {
        maxHp += amount;
    }

    public void RecoverStamina(float amount) {
        if(currentStamina < maxStamina)
            currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        StatusUI.UpdateStaminaSlider(currentStamina / maxStamina);
    }
    public void RecoverSkillGauge(float amount) {
        if(CurrentSP < maxSp)
            CurrentSP = Mathf.Min(CurrentSP + amount, maxStamina);
        StatusUI.UpdateSpSlider(CurrentSP / maxSp);
    }
    
    private void ConsumeStamina(float amount) {
        currentStamina = Mathf.Max(currentStamina - amount, 0);
        StatusUI.UpdateStaminaSlider(currentStamina / maxStamina);
    }
    private void ConsumeSP(float amount) {
        CurrentSP = Mathf.Max(CurrentSP - amount, 0);
        StatusUI.UpdateSpSlider(CurrentSP / maxSp);
    }

    public void CastSkill() {
        if(CurrentSP < activeSkill.Cost)
            return;
        ConsumeSP(activeSkill.Cost);
        GameManager.instance.SoundManager.PlayEffect(GameManager.instance.SoundManager.skillAudioClip);
        GameManager.instance.StageManager.CameraDirector.PlaySkillCinematics();
        activeSkill?.Active();
    }
    
    private void Die() {
        stateMachine.ChangeState(dieState, false);
        StageManager.GameOver();
    }

    public void TurnOnLamp(bool on) {
        StartCoroutine(LampCoroutine(on));
    }
    private IEnumerator LampCoroutine(bool on) {
        float offset = 0;
        float start =   handLampLight.intensity;
        float end =     on ? 0.6f :    0;
        while(offset < 1) {
            handLampLight.intensity = Mathf.Lerp(start, end, offset);
            offset += Time.deltaTime * 0.5f;
            yield return null;
        }
        handLampLight.intensity = end;
    }

    #region IAttachmentsTakeable Implements
    public void TakeAttachment(Attachment attachment) {
        attachment.transform.SetParent(this.transform);
        attachment.transform.localPosition = Vector2.zero;
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