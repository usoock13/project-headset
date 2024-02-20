using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class SkillRanger : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;
    protected override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "Arrow Storm",
            Description: "Ranger's arrows rage like storm."
        );
    }
    protected override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "폭풍 화살",
            Description: "정찰대원의 화살이 폭풍처럼 몰아칩니다."
        );
    }
    #endregion Skill Information

    #region Skill Statue
    public LayerMask targetLayer = 1<<8;
    private float damageCoef = 0.50f;
    private float staticDamage = 25f;
    public float Damage => character.Power * damageCoef + staticDamage;
    
    [SerializeField] private int arrowsPerCycle = 72;
    [SerializeField] private int cycle = 2;
    [SerializeField] private float timeForOneCycle = 0.75f;
    #endregion Skill Statue

    [SerializeField] private GameObject arrowPrefab;
    public ObjectPooler ArrowPooler { get; private set; }

    private Coroutine attackCoroutine;

    [SerializeField] private TrailRenderer trailRenderer;

    private void Awake() {
        ArrowPooler = new ObjectPooler(
            poolingObject: arrowPrefab,
            parent: this.transform,
            count: arrowsPerCycle * cycle,
            restoreCount: arrowsPerCycle * cycle
        );
    }

    public override void Active() {
        trailRenderer.Clear();
        if(attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine() {
        float time = 0;
        float term = timeForOneCycle / arrowsPerCycle;
        Vector2 dir = Vector2.up;

        while(time < timeForOneCycle * cycle) {
            var arrow = ArrowPooler.OutPool(new Vector2(transform.position.x, transform.position.y) + dir, Quaternion.identity);
            arrow.transform.LookAtWithUp(new Vector2(arrow.transform.position.x, arrow.transform.position.y) + dir);

            dir = Quaternion.AngleAxis(360f / arrowsPerCycle, Vector3.forward) * dir;
            time += term;
            yield return new WaitForSeconds(term);
        }
        
    }
}