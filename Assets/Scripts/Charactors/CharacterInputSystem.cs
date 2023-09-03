using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CharacterInputSystem : MonoBehaviour {
    [SerializeField] private Character character;

    private void Awake() {
        character = GetComponent<Character>();
        if(character == null) {
            Debug.LogError("Character Input System failed get Character Component.");
        }
    }
    public void OnMove(InputAction.CallbackContext context) {
        character.SetMoveDirection(context.ReadValue<Vector2>());
    }
    public void OnFixArrow(InputAction.CallbackContext context) {
        if(context.performed) {
            character.FixArrow(true);
        } else if(context.canceled) {
            character.FixArrow(false);
        }
    }
}