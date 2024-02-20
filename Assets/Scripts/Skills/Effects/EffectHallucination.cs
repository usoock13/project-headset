using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHallucination : MonoBehaviour, IDamageable {
    [SerializeField] SkillAdventurer origin;
    private float hp = 100f;
    private int targetLayer = 1<<8;

    [SerializeField] Collider2D tauntArea;
    [SerializeField] ParticleSystem particle;

    public void Active(float hp) {
        this.hp = hp;
        this.gameObject.SetActive(true);
        this.transform.SetParent(null);
        StartCoroutine(TauntCoroutine());
    }

    public void TakeDamage(float amount) {
        hp -= amount;
        if(hp <= 0)
            Disapear();
    }
    public void TakeForce(Vector2 force, float duration = 0.25F) {}
    public void TakeStagger(float second) {}

    private void Disapear() {
        this.gameObject.SetActive(false);
        this.transform.SetParent(origin.transform);
    }

    private IEnumerator TauntCoroutine() {
        while(true) {
            TauntArounds();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void TauntArounds() {
        var inners = new List<Collider2D>();
        var filter = new ContactFilter2D() {
            useTriggers = true,
            useLayerMask = true,
            layerMask = targetLayer,
        };
        Physics2D.OverlapCollider(tauntArea, filter, inners);
        for(int i=0; i<inners.Count; i++) {
            if(inners[i].TryGetComponent(out Monster monster))
                monster.TargetCharacter = transform;
        }
    }
}