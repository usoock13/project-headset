using UnityEngine;

public class EffectBoxSlash : MonoBehaviour {
    private float damage = 15f;
    private float attackDelay = 0.6f;
    [SerializeField] private BoxBounds effectBounds = new BoxBounds(new Vector2(0, 1.5f), new Vector2(3f, 1.5f));
    [SerializeField] private ParticleSystem particle;
    [SerializeField] LayerMask targetLayer;

    private void OnEnable() {
        transform.Translate(effectBounds.center);
        particle.Play();
        Collider2D[] inners = Physics2D.OverlapBoxAll(transform.position, effectBounds.Size, transform.rotation.eulerAngles.z, targetLayer);
        foreach(Collider2D inner in inners) {
            var target = inner.GetComponent<IDamageable>();
            target.TakeDamage(damage);
            target.TakeAttackDelay(attackDelay);
        }
    }
}