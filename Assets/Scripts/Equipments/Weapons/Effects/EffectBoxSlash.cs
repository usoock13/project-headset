using UnityEngine;

public class EffectBoxSlash : MonoBehaviour {
    private float damage = 10f;
    [SerializeField] private BoxBounds effectBounds = new BoxBounds(new Vector2(0, 1.5f), new Vector2(3f, 1.5f));
    [SerializeField] private ParticleSystem particle;

    private void OnEnable() {
        transform.Translate(effectBounds.center);
        particle.Play();
        Collider2D[] inners = Physics2D.OverlapBoxAll(transform.position, effectBounds.Size, transform.rotation.eulerAngles.z);
        foreach(Collider2D inner in inners) {
            var target = inner.GetComponent<IDamageable>();
            target.TakeDamage(damage);
        }
    }
}