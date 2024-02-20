using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWarrior : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;
    protected override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "Blade Storm",
            Description: "Warrior spins with her sword NO MERCY to attack around her."
        );
    }
    protected override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "회전회워리어",
            Description: "전사가 그녀의 검과 함께 자비 없이 회전하여 주변을 공격합니다."
        );
    }
    #endregion Skill Information

    #region Skill Statue
    private LayerMask targetLayer = 1<<8;
    private float damageCoef = 2.0f;
    private float staticDamage = 50f;
    private float DamagePerSecond => character.Power * damageCoef + staticDamage;
    private float attackInterval = 0.15f;
    private float duration = 4f;
    private float attackDelay = 0.5f;
    #endregion Skill Statue

    [SerializeField] private CircleCollider2D attackCollider;

    [SerializeField] private ParticleSystem particle;

    private Coroutine attackCoroutine;

    public override void Active() {
        if(attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        particle.Play();
        
        List<Collider2D> inners = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D() {
            useTriggers = true,
            useLayerMask = true,
            layerMask = targetLayer.value,
        };

        float offset = 0;
        while(offset < duration) {
            yield return new WaitForSeconds(attackInterval);
            offset += attackInterval;

            Physics2D.OverlapCollider(attackCollider, filter, inners);
            for(int i=0; i<inners.Count; i++) {
                if(inners[i].TryGetComponent(out Monster monster)) {
                    monster.TakeDamage(DamagePerSecond * attackInterval);
                    monster.TakeStagger(attackDelay);
                    monster.TakeForce((monster.transform.position - transform.position).normalized * 0.5f, 0.5f);
                }
            }
        }
        particle.Stop();
    }
}