using System;
using UnityEngine;

public class EffectProjectile : MonoBehaviour {
    [SerializeField] protected float flyingTime = 3f;
    [SerializeField] protected float lifetime = 0;

    public Action<EffectProjectile> onDisappear;

    protected virtual void OnEnable() {
        lifetime = 0;
    }
    protected virtual void Update() {
        if(lifetime < flyingTime) {
            lifetime += Time.deltaTime;
        } else {
            Disappear();
        }
    }
    protected virtual void Disappear() {
        onDisappear?.Invoke(this);
    }
}