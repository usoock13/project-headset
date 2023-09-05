using UnityEngine;

[System.Serializable]
public class BoxBounds {
    [SerializeField] public float width;
    [SerializeField] public float height;
    [SerializeField] public Vector2 center;
    
    public BoxBounds(Vector2 center, Vector2 size) {
        this.center = center;
        width = size.x;
        height = size.y;
    }
    public BoxBounds(Vector2 center, float width, float height) {
        this.center = center;
        this.width = width;
        this.height = height;
    }

    public Vector2 Size {
        get { return new Vector2 (width, height); }
    }
}