using System.Collections;
using UnityEngine;

public class ArtifactHandOfGlory : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] staticDamage = new float[] {  15f,   20f,   25f,   30f,   35f };
    private float[] damageCoef = new float[]   { 0.1f,  0.1f,  0.1f,  0.1f,  0.1f };

    public float Damage => staticDamage[level-1] + damageCoef[level-1] * _Character.Power;
    private float characterDamageRatio = .3f;
    public float CharacterDamage => Damage * characterDamageRatio;
    #endregion Artifact Status

    [SerializeField] private EffectGlory explosionOrigin;
    private ObjectPooler explosionPooler;

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Necronomicon",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"When a monster is defeated by the character, the monster explodes and damages other monsters and the <color=#f40>Character</color> around."
                   + $"\n(It damages the character only its 30% amount)"
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"When a monster is defeated by the character, the monster explodes and damages other monsters and the <color=#f40>Character</color> around."
                   + $"\n(It damages the character only its 30% amount)"
                   + $"\n"
                   + $"\nDamage : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "네크로노미콘",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터로 인해 처치된 몬스터가 폭발해 주변 몬스터와 <color=#f40>캐릭터</color>에게 피해를 가합니다."
                   + $"\n(캐릭터에게는 30%의 피해만 가합니다)"
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[0]}+{damageCoef[0]*100}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터로 인해 처치된 몬스터가 폭발해 주변 몬스터와 <color=#f40>캐릭터</color>에게 피해를 가합니다."
                   + $"\n(캐릭터에게는 30%의 피해만 가합니다)"
                   + $"\n"
                   + $"\n피해량 : <color=#f40>{staticDamage[level-1]}+{damageCoef[level-1]*100}%</color> > <color=#f40>{staticDamage[NextLevelIndex]}+{damageCoef[NextLevelIndex]*100}%</color>"
                   + $"</nobr>",
            }
    );
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
