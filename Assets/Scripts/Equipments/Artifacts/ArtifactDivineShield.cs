using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ArtifactDivineShield : Artifact {
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
    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "성스러운 유물함";
    public override string Description =>
        level switch {
            _ => $"피해를 막아주는 보호막을 생성합니다."
               + $"보호막은 피해를 막고 1초 뒤 사라지며 {regenerateInterval[level]}초 뒤에 재생성됩니다."
        };
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