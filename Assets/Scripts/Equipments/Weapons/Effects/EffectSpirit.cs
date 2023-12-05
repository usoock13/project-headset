using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpirit : MonoBehaviour {
    [SerializeField] private WeaponSpirit originWeapon;
    public bool IsActive   { get; private set; } = false;

    private Transform target;
    
    private float maxVelocity = 8f;
    private float battery = 0;
    private float searchRadius = 5f;
    private float forEachAttackInterval = 0.5f;
    private Vector2 currentVelocity;
    private List<GameObject> hitMonsters = new List<GameObject>();

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private SpriteRenderer render;

    private float Damage => originWeapon.Damage;
    private float Acceleration => originWeapon.Acceleration;
    private float hittingDelay = 0.25f;
    private float forceScalar = 0.5f;

    private void Update() {
        if(IsActive) {
            Accelerate();
            battery -= Time.deltaTime;
            if(battery <= 0) {
                Stop();
            }
        }
    }

    public void SearchMonster() { // 01
        StartCoroutine(SearchMonsterCoroutine());
    }

    private IEnumerator SearchMonsterCoroutine() { // 02
        target = null;
        Collider2D[] inners = Physics2D.OverlapCircleAll(originWeapon.transform.position, searchRadius, targetLayer);
        foreach(var inner in inners) {
            if(inner.TryGetComponent(out Monster _)) {
                target = inner.transform;
                Run();
            }
            break;
        }
        if(target is null) {
            yield return new WaitForSeconds(0.5f);
            originWeapon.Waitings.Enqueue(this);
        }
    }

    public void Run() {
        IsActive = true;
        battery = originWeapon.RunningTime;
    }

    public void Stop() { // 03
        IsActive = false;
        hitMonsters.Clear();
        StartCoroutine(ChargeCoroutine());
    }

    private IEnumerator ChargeCoroutine() { // 04
        Vector3 start = transform.position;
        float offset = 0;
        while(offset < 1) {
            transform.position = Vector2.Lerp(start, originWeapon.transform.position, Mathf.Pow(offset, 3));
            offset += Time.deltaTime;
            yield return null;
        }
        transform.position = originWeapon.transform.position;
        yield return new WaitForSeconds(originWeapon.ChargeTime);
        originWeapon.Waitings.Enqueue(this);
    }

    private IEnumerator RemoveHitMonster(GameObject target) {
        yield return new WaitForSeconds(forEachAttackInterval);
        hitMonsters.Remove(target);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(IsActive
        && !hitMonsters.Contains(other.gameObject)
        && 1<<other.gameObject.layer == targetLayer.value) {
            if(other.TryGetComponent(out Monster monster)) {
                monster.TakeDamage(Damage);
                monster.TakeHittingDelay(hittingDelay);
                monster.TakeForce(currentVelocity.normalized * forceScalar, hittingDelay);
                hitMonsters.Add(monster.gameObject);
                StartCoroutine(RemoveHitMonster(monster.gameObject));
            }
        }
    }

    private void Accelerate() {
        Vector2 dir = target.transform.position - transform.position;
        Vector2 next = currentVelocity + dir * Acceleration * Time.deltaTime;
        if(next.magnitude > maxVelocity) {
            next = next.normalized * maxVelocity;
        }
        currentVelocity = next;
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);
    }
}