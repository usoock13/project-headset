using System;
using UnityEngine;

public abstract class Weapon : Equipment {
    protected Character _Character { get { return GameManager.instance.Character; } }
    
    protected abstract float AttackInterval { get; }
    protected float currentTime = 0;
    
    protected virtual void Update() {
        if(currentTime > 0) {
                currentTime -= Time.deltaTime * _Character.AttackSpeed;
        } else {
            while(currentTime <= 0) {
                currentTime += AttackInterval;
                Attack();
                if(AttackInterval <= 0) {
                    Debug.LogWarning("Check the weapon that has 0 attack interval.");
                    break;
                }
            }
        }
    }
    
    protected override void OnLevelUp() {
        base.OnLevelUp();
        GameManager.instance.StageManager.StageUIManager.UpdateWeaponList();
    }
    
    protected abstract void Attack();
    public override void OnGotten() {
        OnEquipped();
    }
    
    public override void OnEquipped() {
        this.gameObject.SetActive(true);
    }
    
    public override void OnTakeOff() {
        this.gameObject.SetActive(false);
    }
}