using System.Collections;
using UnityEngine;

public class SkillMercenary : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;
    public override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "Indomitable",
            Description: "Mercenary get the IMMUNITY from any injuries."
        );
    }
    public override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "불굴",
            Description: "용병이 모든 피해로부터 면역을 얻습니다."
        );
    }
    #endregion Skill Information

    [SerializeField] private float immunityDuration = 4f;
    [SerializeField] private float barrierDuration = 10f;
    [SerializeField] private SpriteRenderer immuneRenderer;
    [SerializeField] private SpriteRenderer barrierRenderer;

    private bool immune = false;
    private float barrier = 0f;
    private float accumulatedDamage = 0f;

    private Coroutine inactiveCoroutine;

    public override void Active() {
        accumulatedDamage = 0;

        character.attackBlocker += MercenarysBlocker;

        immune = true;
        immuneRenderer.enabled = true;

        if(inactiveCoroutine != null)
            StopCoroutine(inactiveCoroutine);
        inactiveCoroutine = StartCoroutine(InactiveCorouine());
    }

    private IEnumerator InactiveCorouine() {
        yield return new WaitForSeconds(immunityDuration);
        immune = false;
        immuneRenderer.enabled = false;
        
        if(accumulatedDamage != 0 ) {
            barrier = accumulatedDamage;
            barrierRenderer.enabled = true;
            yield return new WaitForSeconds(barrierDuration);
            barrierRenderer.enabled = false;
        }
        character.attackBlocker -= MercenarysBlocker;
    }

    private bool MercenarysBlocker(Monster monster, float damage) {
        if(immune) {
            accumulatedDamage += damage;
            GameManager.instance.StageManager.PrintDamageNumber(transform.position, "IMMUNE");
            return true;
        } else {
            if(barrier <= 0)
                return false;
            else {
                barrier -= damage;
                GameManager.instance.StageManager.PrintDamageNumber(transform.position, damage.ToString(), new Color(1f, 0.5f, 0.5f));
                
                monster.TakeDamage(damage * 2f);
                monster.TakeStagger(0.1f);

                if(barrier <= 0)
                    barrierRenderer.enabled = false;

                return true;
            }
        }
    }
}