using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ArtifactHolyBox : Artifact {
    private bool isActive = false;
    private float regenerateCountDown = 0;

    [SerializeField] private SpriteRenderer effectRenderer;

    private Coroutine inactiveCoroutine;

    #region Artifact Status
    const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;
    private float[] regenerateInterval = new float[] { 11.0f,   09.5f,   08.0f,   06.5f,   05.00f };
    private float RegenerateInterval => regenerateInterval[level-1];
    #endregion Artifact Status

    #region Artifact Infromation
    [SerializeField] private Sprite _icon;

    protected override EquipmentInformation InformationEN => new EquipmentInformation(
        Icon: _icon,
        Name: "Reliquary",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"Create a shiled that blocks damage. The shiled will be disapeared in a second after block any damage."
                   + $"\n"
                   + $"\nRegenerating Time : <color=#f40>{regenerateInterval[0]}sec</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"Create a shiled that blocks damage. The shiled will be disapeared in a second after block any damage."
                   + $"\n"
                   + $"\nRegenerating Time : <color=#f40>{regenerateInterval[level-1]}sec</color> > <color=#f40>{regenerateInterval[NextLevelIndex]}sec</color>"
                   + $"</nobr>",
            }
    );
    protected override EquipmentInformation InformationKO => new EquipmentInformation(
        Icon: _icon,
        Name: "성스러운 유물함",
        Description:
            NextLevelIndex switch {
                0 => $"<nobr>"
                   + $"피해를 막아주는 보호막을 생성합니다. 보호막은 어떤 피해든 막게되면 1초 뒤 사라집니다."
                   + $"\n"
                   + $"\n재생성 시간 : <color=#f40>{regenerateInterval[0]}초</color>"
                   + $"</nobr>",
                _ => $"<nobr>"
                   + $"피해를 막아주는 보호막을 생성합니다. 보호막은 어떤 피해든 막게되면 1초 뒤 사라집니다."
                   + $"\n"
                   + $"\n재생성 시간 : <color=#f40>{regenerateInterval[level-1]}초</color> > <color=#f40>{regenerateInterval[NextLevelIndex]}초</color>"
                   + $"</nobr>",
            }
    );
    #endregion Artifact Infromation

    private void Update() {
        if(!isActive) {
            if(regenerateCountDown > 0)
                regenerateCountDown -= Time.deltaTime;
            else
                Active();
        }
    }
    public override void OnEquipped() {
        base.OnEquipped();
        _Character.attackBlocker += TakeAttackHandler;
        Active();
    }
    public override void OnTakeOff() {
        base.OnTakeOff();
        _Character.attackBlocker -= TakeAttackHandler;
        Inactive();
    }

    private void Active() {
        isActive = true;
        effectRenderer.enabled = true;
        regenerateCountDown = RegenerateInterval;
    }
    private void Inactive() {
        isActive = false;
        effectRenderer.enabled = false;
        if(inactiveCoroutine !=null)
            StopCoroutine(inactiveCoroutine);
    }
    public bool TakeAttackHandler(Monster target, float amount) {
        if(isActive) {
            GameManager.instance.StageManager.PrintDamageNumber(transform.position, $"BLOCK", new Color(1f, 1f, .25f));
            inactiveCoroutine = StartCoroutine(InactiveCoroutine());
            return true;
        }
        return false;
    }
    private IEnumerator InactiveCoroutine() {
        yield return new WaitForSeconds(1f);
        Inactive();
    }
}