using UnityEngine;

public interface IDamageable {
    public void TakeDamage(float amount);
    public void TakeHittingDelay(float amount);
    public void TakeForce(Vector2 force);
}