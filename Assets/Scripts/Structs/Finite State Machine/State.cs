using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Rendering;

[System.Serializable]
public class State {
    public delegate void ChangeEvent();
    public delegate void ChangeEventWithState(State state = null);
    public delegate bool ChangeFilter(State current, State next);
    public string stateName { get; protected set; }
    public string stateTag { get; private set; } = null;

    public State() {}
    public State(string stateName) {
        this.stateName = stateName;
    }
    public State(string stateName, string stateTag) {
        this.stateName = stateName;
        this.stateTag = stateTag;
    }

    public ChangeEventWithState onActive;
    public ChangeEvent onStay;
    public ChangeEvent onStayFixed;
    public ChangeEventWithState onInactive;
    
    public ChangeFilter filters;

    public override string ToString() {
        return stateName;
    }
    public bool Compare(State state) {
        return this == state;
    }
    public bool Compare(string stateTag) {
        return this.stateTag == stateTag;
    }
    public bool Changeable(State next) {
        foreach(ChangeFilter filter in filters.GetInvocationList()) {
            if(!filter.Invoke(this, next)) return false;
        }
        return true;
    }
    /* 
        Usage Example >>

        State jumpState = new State("Jump");
        jumpState.OnActive += () => {
            playerGameObject.ownRigidbody.addForce(Vector3.up * jumpPower);
        };
    */
}