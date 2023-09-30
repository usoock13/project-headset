using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;

public abstract class Weapon : Equipment {
    protected Character Character { get { return GameManager.instance.Character; } }
    
    protected abstract float AttackInterval { get; }
    protected float currentTime = 0;
    
    protected void Update() {
        if(currentTime > 0) {
            currentTime -= Time.deltaTime;
        } else {
            currentTime = AttackInterval;
            Attack();
        }
    }
    protected abstract void Attack();
    public override void OnGotten() {
        OnEquipped();
    }
    public override void OnEquipped() {
        this.gameObject.SetActive(true);
    }
}