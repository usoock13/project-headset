using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputSystem : MonoBehaviour {
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Character character;

    private void Awake() {
        character ??= GetComponent<Character>();
        if(character == null) {
            Debug.LogError("'Character Input System' failed to get a Character Component.");
        }
        character.characterInputSystem = this;
    }
    
    public void ChangeToUIMode() {
        playerInput.SwitchCurrentActionMap("UI");
    }
    public void ChangeToControlMode() {
        playerInput.SwitchCurrentActionMap("Character Control");
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
    public void OnDodge(InputAction.CallbackContext context) {
            character.DodgeToward();
    }
    public void ToggleMouse(InputAction.CallbackContext context) {
        if(context.performed)
            character.ToggleMouseAiming();
    }
}