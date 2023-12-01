using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EffectMonkeyExplosion : MonoBehaviour {
    [SerializeField] private ArtifactHandOfGlory originArtifact;
    [SerializeField] private float radius = 2f;

    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private LayerMask characterLayer;

    [SerializeField] ParticleSystem particle;

    private void OnEnable() {
        StartCoroutine(AttackNextFrame());
        particle.Play();
    }
    private void DamageArea() {
        var monsterColliders = Physics2D.OverlapCircleAll(transform.position, radius, monsterLayer.value);
        for(int i=0; i<monsterColliders.Length; i++) {
            if(monsterColliders[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(originArtifact.Damage / 3);
            }
        }
        var characterCollider = Physics2D.OverlapCircle(transform.position, radius, characterLayer.value);
        if(characterCollider != null)
            if(characterCollider.TryGetComponent(out Character character)) {
                character.TakeDamage(originArtifact.CharacterDamage / 3);
            }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    private IEnumerator AttackNextFrame() {
        DamageArea();
        yield return new WaitForSeconds(.05f);
        DamageArea();
        yield return new WaitForSeconds(.05f);
        DamageArea();
    }
}