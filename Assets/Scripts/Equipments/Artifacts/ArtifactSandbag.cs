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
    public override Sprite Icon => _icon;
    public override string Name => "모래주머니";
    public override string Description => 
        level switch {
            _ =>  $"<nobr>획득하면 즉시 이동 속도/공격 속도가 30%/40% 감소합니다.\n"
                + $"적을 처치할 마다 이동 속도/공격 속도가 <color=#f40>{moveSpeedPerStack[level] * 100}%/{attackSpeedPerStack[level] * 100}%</color> 증가하여"
                + $"최대 <color=#f40>{moveSpeedPerStack[level] * 10_000}%/{attackSpeedPerStack[level] * 10_000}%</color>까지 증가합니다."
                + $"<i>최종 이동 속도/공격 속도 <color=#f40>{(int)(moveSpeedPerStack[level] * 30_000 - 30)}%/{attackSpeedPerStack[level] * 30_000 - 40}%</color></i></nobr>"
        };
    #endregion Artifact Information

    public override void OnEquipped() {
        base.OnEquipped();
        _Character.onKillMonster += OnKillMonster;
        _Character.extraMoveSpeed += ExtraMoveSpeed;
        _Character.extraAttackSpeed += ExtraAttackSpeed;
    }
    private void OnKillMonster(Character character, Monster monster) {
        currentStack ++;
        if(currentStack >= MAX_STACK) {
            _Character.onKillMonster -= OnKillMonster;
            currentStack = MAX_STACK;
        }
        extraInformation = currentStack.ToString();
        GameManager.instance.StageManager._StageUIManager.UpdateArtifactList();
    }
    private float ExtraMoveSpeed(Character character) => character.DefaultMoveSpeed * (moveSpeedPerStack[level-1] * currentStack - .3f);
    private float ExtraAttackSpeed(Character character) => attackSpeedPerStack[level-1] * currentStack - .4f;
}
