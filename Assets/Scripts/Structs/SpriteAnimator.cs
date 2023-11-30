using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class SpriteAnimator : MonoBehaviour {
    private Animator _animator;
    public Animator Animator {
        get {
            _animator ??= GetComponent<Animator>();
            return _animator;
        }
    }
    private string _nextAnimationName = null;
    protected bool CurrentAnimationIs(string name) {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    private void Update() {
        if(!CurrentAnimationIs(_nextAnimationName)
        && _nextAnimationName != null)
            if(!StageManager.isGamePause)
                _animator.Play(_nextAnimationName);
    }
    public void ChangeAnimation(string stateName, bool intoSelf=false) {
        if(intoSelf || stateName != _nextAnimationName) {
            _nextAnimationName = stateName;
            
            if(!StageManager.isGamePause)
                _animator.Play(stateName);
        }
    }
    public void SetFloat(string name, float value) {
        _animator.SetFloat(name, value);
    }
}
