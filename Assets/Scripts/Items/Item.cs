using System;
using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour {
    protected bool isActive = false;
    public Action<Item> onGetItem;

    public virtual void Drop() {
        isActive = true;
    }
    public virtual void PickUpItem(Transform getter) {
        if(!isActive)
            StartCoroutine(AbsorbedCoroutine(getter));
    }
    private IEnumerator AbsorbedCoroutine(Transform target) {
        Vector2 origin = (Vector2) transform.position;
        float offset = 0;
        while(offset < 1) {
            transform.position = Vector2.Lerp(origin, target.position, offset*offset);
            offset += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
    protected virtual void OnGetItem() {
        onGetItem?.Invoke(this);
    }
}