using UnityEngine;

[System.Serializable]
public class CircleBounds {
    [SerializeField] public Vector2 center;
    [SerializeField] public float radius;
    
    public CircleBounds(Vector2 center, float radius) {
        this.center = center;
        this.radius = radius;
    }
}