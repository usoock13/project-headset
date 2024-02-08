using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInkStorm : MonoBehaviour {
    private bool isActive = false;

    private LayerMask targetLayer = 1<<3;

    private float damagePerSecond = 50f;
    public float DPS => damagePerSecond;

    private float damageInterval = 0.10f;
    public float Interval => damageInterval;

    private float rotateSpeed = 0;
    private float maxRotateSpeed = 30f;
    private float chargingTime = 2.5f;
    
    [SerializeField] private EffectBranchOfInkStorm[] branches;

    public void Active(float dps) {
        this.damagePerSecond = dps;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        StartCoroutine(ActiveCoroutine());
    }

    public void Inactive() {
        isActive = false;
        for(int i=0; i<branches.Length; i++)
            branches[i].Inactive();
    }

    private IEnumerator ActiveCoroutine() {
        for(int i=0; i<branches.Length; i++)
            branches[i].Active(chargingTime);
            
        yield return new WaitForSeconds(chargingTime);
        isActive = true;
        
        while(isActive) {
            rotateSpeed = rotateSpeed >= maxRotateSpeed ? maxRotateSpeed : rotateSpeed += Time.deltaTime * 10f;
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}