using System;
using UnityEngine;

public class EffectProjectile : MonoBehaviour {
    [SerializeField] protected float flyingTime = 3f;
    [SerializeField] protected float lifetime = 0;

    public Action<EffectProjectile> onDisapear;

    protected virtual void OnEnable() {
        lifetime = 0;
    }
    protected virtual void Update() {
        if(lifetime < flyingTime) {
            lifetime += Time.deltaTime;
        } else {
            Disapear();
        }
    }
    protected virtual void Disapear() {
        onDisapear?.Invoke(this);
    }
}