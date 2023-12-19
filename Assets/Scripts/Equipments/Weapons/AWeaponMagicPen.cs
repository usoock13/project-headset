using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AWeaponMagicPen : Weapon {
    [SerializeField] private EffectAwMagicPen effect;
    [SerializeField] private EffectAwRoughDrawing drawingOrigin;

    [SerializeField] public ObjectPooler DrawingPooler { get; private set; }
    [SerializeField] public ObjectPooler GoblinPooler { get; private set; }

    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] staticDamage = new float[MAX_LEVEL] {   40f,   50f,   60f,   70f,   80f };
    private float[] damageCoef = new float[MAX_LEVEL]   { 0.2f,  0.2f,  0.2f,  0.2f,  0.2f };

    private float[] bombStaticDamage = new float[MAX_LEVEL] { 150f,  200f,  250f,  300f,  400f, };
    private float[] bombDamageCoef = new float[MAX_LEVEL]   { 2.0f,  2.0f,  2.0f,  2.0f,  2.0f, };

    private int[] kesoAmount = new int[MAX_LEVEL] { 30,  50,  70,  90, 110, };
    private int[] kesoCoef = new int[MAX_LEVEL]   {  3,   3,   3,   3,   3, };

    [SerializeField] private EffectPaperGoblin goblinOrigin;
    private float[] goblinStaticDamage = new float[MAX_LEVEL] { 50f,  75f,  100f,  125f,  150f, };
    private float[] goblinDamageCoef = new float[MAX_LEVEL]   {  1f,   1f,    1f,    1f,    1f, };
    private float[] goblinDuration = new float[MAX_LEVEL]     { 15f,  20f,   25f,   30f,   35f, };

    public float Damage => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    public float BombDamage => bombStaticDamage[level-1] + bombDamageCoef[level-1] * _Character.Power;
    public float GoblinDamage => goblinStaticDamage[level-1] + goblinDamageCoef[level-1] * _Character.Power;
    public float GoblinDuration => goblinDuration[level-1];
    public int KesoAmount { get {
        int final = kesoAmount[level-1] + kesoCoef[level-1];
        final = (int) UnityEngine.Random.Range(final * 0.9f, final * 1.1f);
        return final;
    }}
    public float Interval => AttackInterval;
    protected override float AttackInterval => 0.125f;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weapnoIcon;
    public override Sprite Icon => _weapnoIcon;
    public override string Name => "마술 만연필";
    public override string Description => 
        $"<nobr>"
      + $"마술 만연필로 조준 방향을 휘저어 매초 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 가합니다.\n"
      + $"<color=#f40>때때로 걸작이 등장해 도움을 받을 수 있습니다!</color>"
      + "</nobr>";
    #endregion Weapon Information

    [SerializeField] private Transform generatingPoint;
    private readonly int MASTERPIECE_COOLDOWN = 16;
    private int lineOfMasterpiece = 0;

    protected void Awake() {
        DrawingPooler = new ObjectPooler(
            poolingObject: drawingOrigin.gameObject,
            parent: this.transform
        );
        GoblinPooler = new ObjectPooler(
            poolingObject: goblinOrigin.gameObject,
            parent: this.transform
        );
    }

    protected override void Attack() {
        float angle = UnityEngine.Random.Range(-22f, 22f);
        effect.transform.rotation = _Character.attackArrow.rotation;
        effect.transform.Rotate(Vector3.forward, angle);
        effect.AttackForward();
        _Character.OnAttack();
        DrawMasterpiece();
    }

    private void DrawMasterpiece() {
        if(lineOfMasterpiece > 0) {
            lineOfMasterpiece --;
            return;
        }
        float number = UnityEngine.Random.Range(0, 200);
        
        switch(number) {
            case 0:
                DrawingPooler.OutPool(generatingPoint.position, Quaternion.identity)
                    .GetComponent<EffectAwRoughDrawing>()?.Active(EffectAwRoughDrawing.DrawnObject.Bomb);
                lineOfMasterpiece = MASTERPIECE_COOLDOWN;
                break;
            case 1:
                DrawingPooler.OutPool(generatingPoint.position, Quaternion.identity)
                    .GetComponent<EffectAwRoughDrawing>()?.Active(EffectAwRoughDrawing.DrawnObject.FirstAid);
                lineOfMasterpiece = MASTERPIECE_COOLDOWN;
                break;
            case 2:
                DrawingPooler.OutPool(generatingPoint.position, Quaternion.identity)
                    .GetComponent<EffectAwRoughDrawing>()?.Active(EffectAwRoughDrawing.DrawnObject.BoonFoods);
                lineOfMasterpiece = MASTERPIECE_COOLDOWN;
                break;
            case 3:
                DrawingPooler.OutPool(generatingPoint.position, Quaternion.identity)
                    .GetComponent<EffectAwRoughDrawing>()?.Active(EffectAwRoughDrawing.DrawnObject.Keso);
                lineOfMasterpiece = MASTERPIECE_COOLDOWN;
                break;
            case 4:
                DrawingPooler.OutPool(generatingPoint.position, Quaternion.identity)
                    .GetComponent<EffectAwRoughDrawing>()?.Active(EffectAwRoughDrawing.DrawnObject.Goblin);
                lineOfMasterpiece = MASTERPIECE_COOLDOWN;
                break;
        }
    }
}
