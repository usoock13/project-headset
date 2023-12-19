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

    private Transform puller;

    enum ItemState {
        Dropping,
        Dropped,
        Pulled,
        Stored
    }
    ItemState currentState = ItemState.Stored;

    protected virtual void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Drop() {
        StartCoroutine(DropCoroutine());
        gameObject.SetActive(true);
    }
    private IEnumerator DropCoroutine() {
        currentState = ItemState.Dropping;
        
        float randomAngle = UnityEngine.Random.Range(0, 360);
        Vector2 origin = transform.position;
        Vector2 dest = Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector2.up * UnityEngine.Random.Range(.5f, 1.5f);
        float offset = 0;
        while(offset < 1) {
            transform.position = Vector2.Lerp(origin, origin + dest, offset);
            offset += Time.deltaTime * 4f;
            yield return null;
        }
        currentState = ItemState.Dropped;
        if(puller is not null)
            Pull(this.puller);
    }

    public virtual void Pull(Transform puller) {
        if(currentState == ItemState.Dropping) {
            this.puller = puller;
        }
        if(currentState == ItemState.Dropped) {
            pullCoroutine = StartCoroutine(PullCoroutine(puller));
        }
    }
    public virtual void Store(Character onwer) {
        if(!onwer.CurrentState.Compare(onwer.dieState)) {
            OnGotten();
            Disappear();
        }
    }
    protected void Disappear() {
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