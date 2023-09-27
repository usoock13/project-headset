using UnityEngine;

public abstract class Equipment : MonoBehaviour, IDisplayOnUI {
    public abstract Sprite Icon { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public abstract void OnEquipped();
}