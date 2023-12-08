using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponTentacleBag : Weapon {
    [SerializeField] private EffectTentacle tentacleOrigin;
    public ObjectPooler EffectPooler { get; private set; }
    
    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel { get { return MAX_LEVEL; } }
    private float[] interval = new float[MAX_LEVEL]         { 1.50f,   1.50f,   1.20f,   1.20f,   1.00f, }; // 공격 간격
    private float[] staticDamage = new float[MAX_LEVEL]     {    3f,      3f,      5f,      5f,      8f, }; // 추가 피해계수
    private float[] damageCoef = new float[MAX_LEVEL]       { 0.10f,   0.10f,   0.10f,   0.10f,   0.15f, }; // 피해계수
    private int[] effectCount = new int[MAX_LEVEL]          {     1,       2,       2,       3,       3, }; // 촉수 개수
    private float range = 3f;
    [SerializeField] private LayerMask targetLayer = 1<<8;

    protected override float AttackInterval => interval[level-1];
    public float Damage => damageCoef[level-1] * _Character.Power + staticDamage[level-1];
    public int AttackCount => 6;
    private int EffectCount => effectCount[level-1];
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weaponIcon;
    public override Sprite Icon => _weaponIcon;
    public override string Name => "촉수 가방";
    public override string Description =>
        NextLevelIndex switch {
            _ => $"<nobr><color=#f40>{interval[NextLevelIndex]}</color>초에 한 번 근처 적을 향해 촉수가 <color=#f40>{effectCount[NextLevelIndex]}</color>줄기 뻗어나가며 범위 내의 적에게 <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>의 피해를 최대 {AttackCount}회 가합니다.</nobr>",
        };
    #endregion Weapon Information

    private void Awake() {
        EffectPooler = new ObjectPooler(poolingObject: tentacleOrigin.gameObject, parent: this.transform);
    }
    protected override void Attack() {
        StartCoroutine(AttackCoroutine());
        _Character.OnAttack();
    }
    private IEnumerator AttackCoroutine() {
        Collider2D[] inners = Physics2D.OverlapCircleAll(transform.position, range, targetLayer);
        for(int i=0; i<EffectCount; i++) {
            float angle;
            if(i < inners.Length
            && inners[i].TryGetComponent(out Monster monster)) {
                Vector2 targetDir = monster.transform.position - transform.position;
                angle = (Mathf.Atan2(targetDir.y, targetDir.x) - Mathf.PI*0.5f) * Mathf.Rad2Deg;
            } else {
                angle = UnityEngine.Random.Range(0, 360);
            }
            GameObject instance = EffectPooler.OutPool(transform.position + Vector3.up * 0.5f, Quaternion.identity);
            instance.transform.SetParent(this.transform);
            instance.transform.Rotate(Vector3.forward, angle);
            instance.GetComponent<EffectTentacle>()?.Activate();
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}