using UnityEngine;

public abstract class Equipment : MonoBehaviour, IPlayerGettable {
    public string EquipmentType => this.GetType().ToString();

    #region Level
    [SerializeField] protected int level = 0;
    /* 
        When a equipment is gotten, equipment manager increase one its level.
        So Equipment's level is one when their is gotten.
     */
    public int CurrentLevel => level;
    public abstract int MaxLevel { get; }
    #endregion Level

    public abstract Sprite Icon { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }

    public void LevelUp() {
        if(level <= MaxLevel) {
            OnLevelUp();
        } else {
            throw new System.Exception("Level is max.");
        }
    }
    protected virtual void OnLevelUp() {
        level ++;
    }
    public abstract void OnGotten();
    public abstract void OnEquipped();
}