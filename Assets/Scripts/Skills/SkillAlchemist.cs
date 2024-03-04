using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAlchemist : Skill {
    #region Skill Information
    [SerializeField] private Sprite _icon;
    public override SkillInformation InformationEN { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "Reconstruct",
            Description: "Alchemist reconstruct items around her to BOMB."
        );
    }
    public override SkillInformation InformationKO { get => 
        new SkillInformation(
            Icon: _icon,
            Name: "재구성",
            Description: "연금술사가 주변 아이템들을 폭탄으로 재구성합니다."
        );
    }
    #endregion Skill Information

    [SerializeField] private GameObject bombPrefab;
    public ObjectPooler BombPooler { get; private set; }

    [SerializeField] private ParticleSystem particle;

    [SerializeField] private LayerMask targetLayer = 1<<10;

    [SerializeField] private Collider2D areaCollider;
    private float Damage => character.Power * 4.0f + 300f;

    private void Awake() {
        BombPooler = new ObjectPooler(
            poolingObject: bombPrefab,
            count: 50,
            restoreCount: 50
        );
    }

    public override void Active() {
        var inners = new List<Collider2D>();
        var filter = new ContactFilter2D() {
            useLayerMask = true,
            useTriggers = true,
            layerMask = targetLayer.value
        };
        Physics2D.OverlapCollider(areaCollider, filter, inners);
        var targets = new GameObject[inners.Count];
        for(int i=0; i<inners.Count; i++)
            targets[i] = inners[i].gameObject;

        StartCoroutine(ReconstructItems(targets));
    }
    
    private IEnumerator ReconstructItems(GameObject[] items) {
        particle.Play();
        for(int i=0; i<items.Length; i++) {
            items[i].GetComponent<Item>()?.Disappear();
            var sprite = items[i].GetComponent<SpriteRenderer>().sprite;
            var bomb = BombPooler.OutPool();
            bomb.transform.position = items[i].transform.position;
            bomb.GetComponent<EffectItemBomb>().Active(sprite, Damage);

            yield return new WaitForSeconds(0.05f);
        }
    }
}