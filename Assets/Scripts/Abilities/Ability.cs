using UnityEngine;

public abstract class Ability : MonoBehaviour {
    public abstract Sprite Icon { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public abstract void OnTaken(Character character);
    public abstract void OnReleased(Character character);
}