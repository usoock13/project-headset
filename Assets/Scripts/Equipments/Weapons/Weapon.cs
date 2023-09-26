using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;

public abstract class Weapon : Equipment {
    protected Character Character { get { return GameManager.instance.Character; } }
    
    #region Weapon Status
    [SerializeField] protected int level = 0;
    protected abstract int MaxLevel { get; }
    protected abstract float AttackInterval { get; }
    #endregion Weapon Status

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
    public virtual void LevelUp() {
        if(level < MaxLevel) {
            level++;
        } else {
            throw new System.Exception("Level is max.");
        }
    }
public override void OnEquipped() {
        this.gameObject.SetActive(true);
    }
}