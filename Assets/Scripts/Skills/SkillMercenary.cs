using System.Collections;
using UnityEngine;

public class SkillMercenary : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;
    public override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "Ecstasy",
            Description: "Mercenary loses aiming ability and gets a VERY high attack speed for a short time."
        );
    }
    public override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "무아지경",
            Description: "용병이 잠깐동안 조준 능력을 상실하고 매우 높은 공격 속도를 얻습니다."
        );
    }
    #endregion Skill Information

    [SerializeField] private float duration = 4f;
    [SerializeField] private ParticleSystem effectParticle;

    private Coroutine inactiveCoroutine;

    public override void Active() {
        if(inactiveCoroutine != null)
            StopCoroutine(inactiveCoroutine);
        inactiveCoroutine = StartCoroutine(InactiveCorouine());
    }

    private IEnumerator InactiveCorouine() {
        float time = 0;
        Vector2 dir = Vector2.up;
        character.extraAttackSpeed += GetExtraAttackSpeed;
        character.LockAttackArrow(true);
        while(time < duration) {
            dir = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * dir;
            character.RotateArrow(dir);
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
        }
        character.LockAttackArrow(false);
        character.extraAttackSpeed -= GetExtraAttackSpeed;
    }
    
    private float GetExtraAttackSpeed(Character character) => 9f;
}