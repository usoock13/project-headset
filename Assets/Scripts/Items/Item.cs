using System;
using System.Collections;
using UnityEngine;

public abstract class Item : MonoBehaviour, IPlayerGettable {
    public Action<Item> onGetItem;

    public abstract Sprite Icon { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }

    protected SpriteRenderer spriteRenderer;

    #region States
    #endregion States

    private Coroutine pullCoroutine;

    enum ItemState {
        Dropped,
        Pulled,
        Stored
    }
    ItemState currentState = ItemState.Stored;

    protected void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Drop() {
        currentState = ItemState.Dropped;
        gameObject.SetActive(true);
    }
    public virtual void Pull(Transform getter) {
        if(currentState == ItemState.Dropped) {
            pullCoroutine = StartCoroutine(PullCoroutine(getter));
        }
    }
    public virtual void Store(Character onwer) {
        if(!onwer.CurrentState.Compare(onwer.dieState)) {
            OnGotten();
            Disapear();
        }
    }
    protected void Disapear() {
        gameObject.SetActive(false);
        currentState = ItemState.Stored;
        StopAllCoroutines();
    }
    private IEnumerator PullCoroutine(Transform target) {
        Vector2 origin = (Vector2) transform.position;
        float offset = 0;
        while(offset < 2) {
            transform.position = Vector2.Lerp(origin, target.position, offset*offset);
            offset += Time.deltaTime;
            yield return null;
        }
        currentState = ItemState.Dropped;
    }
    public virtual void OnGotten() {
        onGetItem?.Invoke(this);
        GameManager.instance.StageManager.OnGetItem(this);
    }
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if(currentState == ItemState.Dropped
        || currentState == ItemState.Pulled) {
            if(other.TryGetComponent<Character>(out var ch)) {
                Store(ch);
            }
        }
    }
}