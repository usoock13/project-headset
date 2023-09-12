using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;

public abstract class Weapon : MonoBehaviour {
    protected Character character;
    
    #region Weapon Status
    protected int level = 0;
    protected abstract int MaxLevel { get; }
    protected abstract float AttackInterval { get; }
    #endregion Weapon Status

    #region Weapon Information
    public abstract Sprite Icon { get; }
    public abstract string WeaponName { get; }
    public abstract string Description { get; }
    #endregion Weapon Information

    protected float currentTime = 0;

    protected virtual void Awake() {
        character = GameManager.instance.Character;
    }
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
}