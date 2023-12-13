using UnityEngine;

public class ItemCollector : MonoBehaviour {
    // private CircleCollider2D rangeCollider;
    // public float Radius {
    //     get => rangeCollider.radius; 
    //     set { rangeCollider.radius = value; }
    // }

    // private void Start() {
        // rangeCollider = GetComponent<CircleCollider2D>();
    // }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Item"
        && other.TryGetComponent(out Item item)) {
            item.Pull(transform);
        }
    }
}