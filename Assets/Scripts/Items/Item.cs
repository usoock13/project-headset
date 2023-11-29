using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public abstract class Item : MonoBehaviour, IPlayerGettable {
    public Action<Item> onGetItem;

    public abstract Sprite Icon { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }

    protected StateMachine stateMachine;
    protected SpriteRenderer spriteRenderer;

    #region States
    protected State inactiveState = new State("Inactive");
    protected State droppedState = new State("Dropped");
    protected State pickingUpState = new State("Picking Up");
    protected State storedState = new State("Stores");
    #endregion States

    private Coroutine pickUpCoroutine;

    protected void Awake() {
        stateMachine = GetComponent<StateMachine>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeStates();
    }
    protected virtual void InitializeStates() {
        stateMachine.SetIntialState(inactiveState);
        droppedState.onActive += (State previous) => {
            gameObject.SetActive(true);
        };
        pickingUpState.onInactive += (State next) => {
            StopCoroutine(pickUpCoroutine);
        };
        storedState.onActive += (State previous) => {
            gameObject.SetActive(false);
        };
    }
    public virtual void Drop() {
        stateMachine.ChangeState(droppedState);
    }
    public virtual void PickUpItem(Transform getter) {
        if(stateMachine.Compare(droppedState)) {
            stateMachine.ChangeState(pickingUpState);
            pickUpCoroutine = StartCoroutine(PickUpCoroutine(getter));
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
        stateMachine.ChangeState(droppedState);
    }
    public virtual void OnGotten() {
        onGetItem?.Invoke(this);
        GameManager.instance.Character.onGetItem?.Invoke(this);
    }
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if(stateMachine.Compare(droppedState)
        || stateMachine.Compare(pickingUpState)) {
            if(other.TryGetComponent<Character>(out var ch)) {
                if(ch.CurrentState.Compare(ch.dieState))
                    return;
                OnGotten();
                stateMachine.ChangeState(storedState);
            }
        }
    }
}