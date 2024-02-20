using System;
using UnityEngine;

public abstract class EffectProjectile : MonoBehaviour {
    [SerializeField] protected float flyingTime = 3f;
    [SerializeField] protected float lifetime = 0;

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
    protected abstract void Disappear();
}