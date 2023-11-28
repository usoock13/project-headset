using UnityEngine;

public class ItemCollector : MonoBehaviour {
    [SerializeField] public Transform target;
    
    private CircleCollider2D rangeCollider;
    public float Radius {
        get => rangeCollider.radius; 
        set { rangeCollider.radius = value; }
    }

    private void Start() {
        rangeCollider = GetComponent<CircleCollider2D>();
    }
    private void Update() {
        transform.position = target.position;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Item item;
        if(other.tag == "Item"
        && other.TryGetComponent<Item>(out item)) {
            item.PickUpItem(transform);
            GameManager.instance.Character.onGetItem?.Invoke(item);
        }
    }
}