using UnityEngine;

public interface IDisplayOnUI {
    #region Information for UI
    public abstract Sprite Icon { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    #endregion Information for UI
}