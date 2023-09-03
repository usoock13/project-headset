using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;

public abstract class Weapon : MonoBehaviour {
    [SerializeField] protected float maxDelay;
    protected float currentDelay;

    protected void Update() {
        if(currentDelay > 0) {
            currentDelay -= Time.deltaTime;
        } else {
            currentDelay = maxDelay;
            Attack();
        }
    }
    protected abstract void Attack();
}