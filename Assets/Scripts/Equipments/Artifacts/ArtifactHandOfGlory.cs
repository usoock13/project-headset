using System.Collections;
using UnityEngine;

public class ArtifactHandOfGlory : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] damage = new float[] { 15f, 20f, 25f, 30f, 35f };

    public float Damage => damage[level-1];
    private float characterDamageRatio = .3f;
    public float CharacterDamage => Damage * characterDamageRatio;
    #endregion Artifact Status

    [SerializeField] private EffectGlory explosionOrigin;
    private ObjectPooler explosionPooler;

    #region Artifact Information
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "영광의 손";
    public override string Description => 
        NextLevelIndex switch {
            _ => $"<nobr>죽은 적이 폭발하여 주변에 <color=#f40>{damage[NextLevelIndex]}</color>의 피해를 가합니다. <color=#f40>캐릭터도 폭발에 피해를 입을 수 있습니다!</color></nobr>"
        };
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        explosionPooler = new ObjectPooler(poolingObject: explosionOrigin.gameObject, parent: this.transform);
        _Character.onKillMonster += GenerateBoom;
    }

    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.onKillMonster -= GenerateBoom;
    }

    private void GenerateBoom(Character character, Monster deadMonster) {
        Vector2 point = deadMonster.transform.position;
        GameObject instance = explosionPooler.OutPool(point, Quaternion.identity);
        StartCoroutine(InPoolCoroutine(instance));
    }

    private IEnumerator InPoolCoroutine(GameObject instance) {
        yield return new WaitForSeconds(4f);
        explosionPooler.InPool(instance);
    }
}
