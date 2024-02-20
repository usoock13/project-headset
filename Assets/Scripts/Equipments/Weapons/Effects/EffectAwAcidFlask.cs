using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EffectAwAcidFlask : EffectProjectile {
    private bool isFlying = false;
    [SerializeField] private AWeaponAcidFlask originWeapon;

    [SerializeField] private ParticleSystem explosionParticle;

    private float flyingSpeed = 45f;
    private float currentSpeed = 0f;

    [SerializeField] private LayerMask targetLayer = 1<<8;
    [SerializeField] new private SpriteRenderer renderer;

    protected override void OnEnable() {
        base.OnEnable();
        isFlying = true;
        renderer.enabled = true;
    }
    protected override void Update() {
        if(isFlying) {
            transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
            currentSpeed = Mathf.Lerp(flyingSpeed, 0, lifetime/flyingTime);
            base.Update();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isFlying
        && (1<<other.gameObject.layer & targetLayer.value) > 0) {
            if(other.TryGetComponent(out Monster _)) {
                Disappear();
            }
        }
    }

    protected override void Disappear() {
        isFlying = false;
        explosionParticle.Play();
        renderer.enabled = false;
        originWeapon.CloudPooler.OutPool(this.transform.position, Quaternion.identity);
    }
    private IEnumerator InPoolCoroutine() {
        yield return new WaitForSeconds(3f);
        originWeapon.FlaskPooler.InPool(this.gameObject);
    }
}
