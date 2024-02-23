using System.Collections;
using UnityEngine;

public class EffectBlessingArea : MonoBehaviour {
    [SerializeField] private SkillPriest origin;
    [SerializeField] private ParticleSystem areaParticle;

    [SerializeField] private float duration = 8f;
    [SerializeField] private float radius = 5f;
    private float healInterval = 0.5f;

    private Coroutine healCoroutine;
    
    public void Active() {
        if(healCoroutine != null)
            StopCoroutine(healCoroutine);
        healCoroutine = StartCoroutine(HealCoroutine());
    }

    private IEnumerator HealCoroutine() {
        areaParticle.Play();
        float time = 0;
        while(time < duration) {
            if(Vector2.Distance(transform.position, origin.Character.transform.position) < radius)
                origin.Character.TakeHeal(origin.HealPerSecond * healInterval);
            time += healInterval;
            yield return new WaitForSeconds(healInterval);
        }
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}