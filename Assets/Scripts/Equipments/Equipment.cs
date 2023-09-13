using UnityEngine;

public abstract class Equipment : MonoBehaviour {
    #region Information for UI
    public abstract Sprite Icon { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    #endregion Information for UI

    public abstract void OnEquipped();
}