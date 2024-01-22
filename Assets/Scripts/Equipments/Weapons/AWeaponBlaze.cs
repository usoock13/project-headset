using System.Collections;
using UnityEngine;

public class AWeaponBlaze : Weapon {
    [SerializeField] private EffectAwFireball[] effects;
    [SerializeField] private EffectAwFireArrow fireArrow;
    public ObjectPooler ArrowPooler { get; private set; }

    public ObjectPooler BurningPooler { get; private set; }
    public ObjectPooler FreezePooler { get; private set; }
    public ObjectPooler ShockPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] staticDamage    = new float[MAX_LEVEL] {   50f,     60f,    70f,     80f,     90f }; // 고정 피해량
    private float[] damageCoef      = new float[MAX_LEVEL] { 0.55f,   0.55f,  0.55f,   0.55f,   0.55f }; // 피해 계수

    private float[] arrowStaticDamage = new float[MAX_LEVEL] {   25f,    30f,    35f,    40f,    45f, }; // 불화살 고정 피해량
    private float[] arrowDamageCoef   = new float[MAX_LEVEL] { 0.15f,  0.15f,  0.15f,  0.15f,  0.15f, }; // 불화살 피해 계수

    protected override float AttackInterval => 999;
    public float ReactiveInterval => 1.5f;
    public int NumberOfArrow => 6;
    public float Damage => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    public float ArrowDamage => arrowStaticDamage[level-1] + arrowDamageCoef[level-1] * _Character.Power;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "Blaze",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Now fireball explodes larger. When fireball explodes, several fire arrows appears from the explosion."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\nTime of Regenerate : <color=#f40>{ReactiveInterval}sec</color>"
                   + $"\nArrow Damage : <color=#f40>{arrowStaticDamage[0]}+{arrowDamageCoef[0]*100}%</color>"
                   + $"\nNumber of Arrow : <color=#f40>{NumberOfArrow}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Now fireball explodes larger. When fireball explodes, several fire arrows appears from the explosion."
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nTime of Regenerate : <color=#f40>{ReactiveInterval}sec</color>"
                   + $"\nArrow Damage : <color=#f40>{arrowStaticDamage[level-1]}+{arrowDamageCoef[level-1]*100}%</color> > <color=#f40>{arrowStaticDamage[NextLevelIndex]}+{arrowDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\nNumber of Arrow : <color=#f40>{NumberOfArrow}</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _weaponIcon,
        Name: "블레이즈",
        Description: 
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"이제 화염구가 더 넓게 폭발합니다. 화염구가 폭발할 때 화염 화살이 여러개 생성 됩니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"\n재생성 시간 : <color=#f40>{ReactiveInterval}초</color>"
                   + $"\n화살 피해량 : <color=#f40>{arrowStaticDamage[0]}+{arrowDamageCoef[0]*100}%</color>"
                   + $"\n화살 개수 : <color=#f40>{NumberOfArrow}</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"이제 화염구가 더 넓게 폭발합니다. 화염구가 폭발할 때 화염 화살이 여러개 생성 됩니다."
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n재생성 시간 : <color=#f40>{ReactiveInterval}초</color>"
                   + $"\n화살 피해량 : <color=#f40>{arrowStaticDamage[level-1]}+{arrowDamageCoef[level-1]*100}%</color> > <color=#f40>{arrowStaticDamage[NextLevelIndex]}+{arrowDamageCoef[NextLevelIndex]*100}%</color>"
                   + $"\n화살 개수 : <color=#f40>{NumberOfArrow}</color>"
                   + $"</nobr>",
            }
    );
    #endregion Weapon Information
    
    protected override void Update() {}
    protected override void Attack() {}

    private void Awake() {
        ArrowPooler = new ObjectPooler(
            poolingObject: fireArrow.gameObject,
            count: 50,
            parent: this.transform
        );
    }

    public override void OnEquipped() {
        base.OnEquipped();
        for(int i=0; i<effects.Length; i++) {
            effects[i].gameObject.SetActive(true);
            effects[i].Active();
        }
    }
    protected override void OnLevelUp() {
        base.OnLevelUp();
        for( int i=0; i<(level>=5?6:3); i++ ) {
            effects[i].gameObject.SetActive(true);
            effects[i].Active();
        }
    }
}