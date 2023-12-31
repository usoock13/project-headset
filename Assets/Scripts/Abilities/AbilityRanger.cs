using System;
using System.Collections;
using UnityEngine;

public class AbilityRanger : Ability {
    private Sprite icon;

    public override Sprite Icon => icon;
    public override string Name => "호다닥";
    public override string Description => "<nbr>피해를 받으면 짧은 시간동안 이동속도가 증가합니다.</nbr>";

    private bool isActive = false;
    private float duration = 1.5f;
    private float extraMoveSpeed = 2.5f;
    
    private Coroutine durationCoroutine;

    public override void OnTaken(Character character) {
        character.onTakeDamage += OnTakeDamage;
        character.extraMoveSpeed += GetExtraMoveSpeed;
    }
    public override void OnReleased(Character character) {
        character.onTakeDamage -= OnTakeDamage;
        character.extraMoveSpeed -= GetExtraMoveSpeed;
    }
    private void OnTakeDamage(Character character, float amount) {
        if(durationCoroutine != null)
            StopCoroutine(durationCoroutine);
        durationCoroutine = StartCoroutine(DurationCoroutine(character));
    }
    private IEnumerator DurationCoroutine(Character character) {
        isActive = true;
        yield return new WaitForSeconds(duration);
        isActive = false;
    }
    private float GetExtraMoveSpeed(Character character) {
        return isActive ? extraMoveSpeed : 0;
    }
}