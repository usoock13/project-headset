using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMagicDrawing : MonoBehaviour {
    [SerializeField] private AWeaponMagicPen originWeapon;
    
    private Coroutine attackCoroutine;

    public void AttackForward() {
        if(attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }
    public IEnumerator AttackCoroutine() {
        transform.Translate(Vector2.up);
        yield return new WaitForSeconds (originWeapon.Interval * 0.5f);
        transform.position = originWeapon.transform.position;
    }
}
