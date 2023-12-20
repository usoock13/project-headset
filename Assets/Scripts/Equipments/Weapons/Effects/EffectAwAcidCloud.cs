using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAwAcidCloud : MonoBehaviour {
    [SerializeField] private AWeaponAcidFlask originWeapon;

    [SerializeField] private LayerMask targetLayer = 1<<8;
    [SerializeField] private ParticleSystem cloudParticle;
    [SerializeField] new private Collider2D collider;
    
    private void OnEnable() {
        cloudParticle.Play();
        StartCoroutine(AttachCoroutine());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    private IEnumerator AttachCoroutine() {
        float lifetime = 0;
        float duration = originWeapon.cloudDuration;
        while(lifetime < duration) {
            var inners = new List<Collider2D>();
            var filter = new ContactFilter2D() {
                layerMask = targetLayer.value,
            };
            Physics2D.OverlapCollider(collider, filter, inners);
            
            for(int i=0; i<inners.Count; i++) {
                if(inners[i].TryGetComponent(out Monster monster)) {
                    if(monster.TryGetAttachment(out AttachmentCorrosion c)) {
                        c.Duplicate();
                    } else {
                        var attachment = originWeapon.AttachmentPooler.OutPool().GetComponent<AttachmentCorrosion>();
                        monster.TakeAttachment(attachment);
                    }
                }
            }
            lifetime += 1f;
            yield return new WaitForSeconds(1f);
        }
    }
}
