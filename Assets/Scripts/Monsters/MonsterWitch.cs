using System.Collections;
using UnityEngine;

public class MonsterWitch : Monster {
    public override string MonsterType => "Boss Witch";


    protected override void InitializeStates() {
        base.InitializeStates();
    }

    public override void TakeDamage(float amount) {
        base.TakeDamage(amount);
    }

    public override void TakeAttackDelay(float second) {
        StartCoroutine(TakeBitDelayCoroutine());
    }

    public override void TakeForce(Vector2 force, float duration) {
        base.TakeForce(force * 0.1f, duration);
    }
}