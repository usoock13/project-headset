using System;
using System.Collections;
using UnityEngine;

public abstract class Item : MonoBehaviour {
    protected bool isDropped = false;
    protected bool hasPickUp = false;
    public Action<Item> onGetItem;

    public virtual void Drop() {
        isDropped = false;
    }
    public virtual void PickUpItem(Transform getter) {
        if(!isDropped) {
            isDropped = true;
            StartCoroutine(PickUpCoroutine(getter));
        }
    }
    private IEnumerator PickUpCoroutine(Transform target) {
        Vector2 origin = (Vector2) transform.position;
        float offset = 0;
        while(offset < 2) {
            transform.position = Vector2.Lerp(origin, target.position, offset*offset);
            offset += Time.deltaTime;
            yield return null;
        }
        isDropped = false;
    }
    public virtual void OnGetItem() {
        onGetItem?.Invoke(this);
        this.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Character character;
        if(other.TryGetComponent<Character>(out character)) {
            character.GetItem(this);
        }
    }
}