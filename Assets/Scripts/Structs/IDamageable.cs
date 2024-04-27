using UnityEngine;

public interface IDamageable {
    public void TakeDamage(float amount, GameObject origin=null);
    public void TakeStagger(float second, GameObject origin=null);
    public void TakeForce(Vector2 force, float duration = .25f, GameObject origin=null);
}