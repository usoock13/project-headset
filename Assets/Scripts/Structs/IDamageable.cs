using UnityEngine;

public interface IDamageable {
    public void TakeDamage(float amount);
    public void TakeHittingDelay(float second);
    public void TakeForce(Vector2 force, float duration = .25f);
}