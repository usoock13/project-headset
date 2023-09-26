using UnityEngine;

public class ItemGetter : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        Item item;
        if(other.tag == "Item"
        && other.TryGetComponent<Item>(out item)) {
            item.PickUpItem(transform);
        }
    }
}