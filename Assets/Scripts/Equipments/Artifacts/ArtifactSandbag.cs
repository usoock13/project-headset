using System.Collections;
using UnityEngine;

public class ArtifactSandbag : Artifact {
    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private const int MAX_STACK = 300;
    private int currentStack = 0;
    private float[] moveSpeedPerStack = new float[]     { 0.00_10f,   0.00_12f,   0.00_14f,   0.00_16f,   0.00_18f };
    private float[] attackSpeedPerStack = new float[]   { 0.00_15f,   0.00_18f,   0.00_21f,   0.00_24f,   0.00_27f };
    #endregion Artifact Status

    #region Artifact Information
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Sandbag",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Decrease the character's <color=#f40>Movement Speed/Attack Speed</color>."
                   + $"\nEach time when the character defeats a monster, get increasing <color=#f40>Movement Speed/Attack Speed</color>."
                   + $"\n"
                   + $"\nDecreasing Amount : <color=#f40>30%/40%</color>"
                   + $"\nIncreasing Amount : <color=#f40>{moveSpeedPerStack[0]*100}%/{attackSpeedPerStack[0]*100}%</color>"
                   + $"\nMaximum Increasing : <color=#f40>{moveSpeedPerStack[0]*100 * MAX_STACK}%/{attackSpeedPerStack[0]*100 * MAX_STACK}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Decrease the character's <color=#f40>Movement Speed/Attack Speed</color>."
                   + $"\nEach time when the character defeats a monster, get increasing <color=#f40>Movement Speed/Attack Speed</color>."
                   + $"\n"
                   + $"\nDecreasing Amount : <color=#f40>30%/40%</color>"
                   + $"\nIncreasing Amount : <color=#f40>{moveSpeedPerStack[level-1]*100}%/{attackSpeedPerStack[level-1]*100}%</color> > <color=#f40>{moveSpeedPerStack[NextLevelIndex]*100}% / {attackSpeedPerStack[NextLevelIndex]*100}%</color>"
                   + $"\nMaximum Increasing : <color=#f40>{moveSpeedPerStack[level-1]*100 * MAX_STACK}%/{attackSpeedPerStack[level-1]*100 * MAX_STACK}%</color> > <color=#f40>{moveSpeedPerStack[NextLevelIndex]*100 * MAX_STACK}% / {attackSpeedPerStack[NextLevelIndex]*100 * MAX_STACK}%</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "모래주머니",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"캐릭터의 <color=#f40>이동 속도/공격 속도</color>가 감소합니다."
                   + $"\n이후 몬스터를 처치할 때마다 <color=#f40>이동 속도/공격 속도</color>가 증가합니다."
                   + $"\n"
                   + $"\n감소량 : <color=#f40>30%/40%</color>"
                   + $"\n증가량 : <color=#f40>{moveSpeedPerStack[0]*100}%/{attackSpeedPerStack[0]*100}%</color>"
                   + $"\n최대 증가량 : <color=#f40>{moveSpeedPerStack[0]*100 * MAX_STACK}%/{attackSpeedPerStack[0]*100 * MAX_STACK}%</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"캐릭터의 <color=#f40>이동 속도/공격 속도</color>가 감소합니다."
                   + $"\n이후 몬스터를 처치할 때마다 <color=#f40>이동 속도/공격 속도</color>가 증가합니다."
                   + $"\n"
                   + $"\n감소량 : <color=#f40>30%/40%</color>"
                   + $"\n증가량 : <color=#f40>{moveSpeedPerStack[level-1]*100}%/{attackSpeedPerStack[level-1]*100}%</color> > <color=#f40>{moveSpeedPerStack[NextLevelIndex]*100}% / {attackSpeedPerStack[NextLevelIndex]*100}%</color>"
                   + $"\n최대 증가량 : <color=#f40>{moveSpeedPerStack[level-1]*100 * MAX_STACK}%/{attackSpeedPerStack[level-1]*100 * MAX_STACK}%</color> > <color=#f40>{moveSpeedPerStack[NextLevelIndex]*100 * MAX_STACK}% / {attackSpeedPerStack[NextLevelIndex]*100 * MAX_STACK}%</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.onKillMonster += OnKillMonster;
        _Character.extraMoveSpeed += ExtraMoveSpeed;
        _Character.extraAttackSpeed += ExtraAttackSpeed;
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.onKillMonster -= OnKillMonster;
        _Character.extraMoveSpeed -= ExtraMoveSpeed;
        _Character.extraAttackSpeed -= ExtraAttackSpeed;
    }
    private void OnKillMonster(Character character, Monster monster) {
        currentStack ++;
        if(currentStack >= MAX_STACK) {
            _Character.onKillMonster -= OnKillMonster;
            currentStack = MAX_STACK;
        }
        extraInformation = currentStack.ToString();
        GameManager.instance.StageManager.StageUIManager.UpdateArtifactList();
    }
    private float ExtraMoveSpeed(Character character) => character.DefaultMoveSpeed * (moveSpeedPerStack[level-1] * currentStack - .3f);
    private float ExtraAttackSpeed(Character character) => attackSpeedPerStack[level-1] * currentStack - .4f;
}
