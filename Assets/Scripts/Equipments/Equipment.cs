using System;
using UnityEngine;
using Utility;

namespace System.Runtime.CompilerServices {
    internal static class IsExternalInit {};
}

public abstract class Equipment : MonoBehaviour, IPlayerGettable {
    public string EquipmentType => this.GetType().ToString();

    protected record EquipmentInformation(Sprite Icon, string Name, string Description);

    protected abstract EquipmentInformation InformationEN { get; }
    protected abstract EquipmentInformation InformationKO { get; }

    #region Level
    [SerializeField] protected int level = 0;
    /* 
        When a equipment is gotten, equipment manager increase one its level.
        So Equipment's level is one when their is gotten.
     */
    public int CurrentLevel => level;
    public abstract int MaxLevel { get; }
    public bool IsMaxLevel => CurrentLevel>=MaxLevel;
    protected int NextLevelIndex => CurrentLevel < MaxLevel ? level : level-1;
    #endregion Level

    public Sprite Icon { get {
        return GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.Icon,
             "Korean (ko)" => InformationKO.Icon,
             
                        _  => InformationEN.Icon,
        };
    }}
    public string Name { get {
        return GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.Name,
             "Korean (ko)" => InformationKO.Name,
             
                        _  => InformationEN.Name,
        };
    }}
    public string Description { get {
        return GameManager.instance.SelectedLocale.LocaleName switch {
            "English (en)" => InformationEN.Description,
             "Korean (ko)" => InformationKO.Description,
             
                        _  => InformationEN.Description,
        };
    }}
    public string extraInformation = "";

    public void LevelUp() {
        if(level < MaxLevel) {
            OnLevelUp();
        } else {
            throw new System.Exception("Level is max.");
        }
    }
    protected virtual void OnLevelUp() {
        level ++;
    }
    public abstract void OnGotten();   // This method is called when character get this. It's necessary for be displayed on UI.
    public abstract void OnEquipped(); // This method is called by 'OnGotten' method.
    public abstract void OnTakeOff();
    public override string ToString() {
        return this.Name;
    }
}