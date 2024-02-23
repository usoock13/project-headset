using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFinalPortal : MonoBehaviour {
    [SerializeField] private SkillMage origin;
    
    [SerializeField] private LayerMask targetLayer = 1<<8;
    [SerializeField] private Collider2D attackCollider;

    private float delayTime = 1.5f;
    private float duration = 5f;
    private float attackInterval = 0.15f;
    private float damagePerSecond => origin.DamagePerSecond;

    [SerializeField] private ParticleSystem portalParticle;
    [SerializeField] private ParticleSystem sparkParticle;

    private Coroutine attackCoroutine;
    

    public void Active() {
        transform.SetParent(null);
        gameObject.SetActive(true);
        sparkParticle.Stop();
        
        if(attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    public void Inactive() {
        gameObject.SetActive(false);
        origin.PortalPooler.InPool(this.gameObject);
    }
    
    private IEnumerator AttackCoroutine () {
        portalParticle.Play(false);
        yield return new WaitForSeconds(delayTime);
        GameManager.instance.StageManager.CameraDirector.ShakeCamera(5, 1);
        sparkParticle.Play();

        float time = 0f;
        while(time < duration) {
            var inners = new List<Collider2D>();
            var filter = new ContactFilter2D() {
                useTriggers = true,
                useLayerMask = true,
                layerMask = targetLayer
            };

            Physics2D.OverlapCollider(attackCollider, filter, inners);
            for(int i=0; i<inners.Count; i++) {
                if(inners[i].TryGetComponent(out Monster monster)) {
                    monster.TakeDamage(damagePerSecond * attackInterval);
                    monster.TakeStagger(0.2f);
                    monster.TakeForce(transform.up * 0.15f, 0.5f);
                }
            }
            time += attackInterval;
            yield return new WaitForSeconds(attackInterval);
        }
        sparkParticle.Stop();
        yield return new WaitForSeconds(3f);
        Inactive();
    }
}