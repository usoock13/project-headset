using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChainSickle : MonoBehaviour {
    public WeaponChainSickle originWeapon;

    private float flyingSpeed = 24f;
    private float flyingTime = 0.3f;
    private float flyingHittingDelay = 0.5f;

    private bool isPulling = false;
    private float pullingTime = 0.8f;
    private float pullingHittingDelay = 0.8f;

    private List<GameObject> hitMonsters = new List<GameObject>();
    [SerializeField] private LayerMask targetLayer = 1<<8;
    [SerializeField] private Transform pivot;
    [SerializeField] private Animation anim;
    [SerializeField] private AnimationClip flyingAnimation;
    [SerializeField] private AnimationClip pullingAnimation;
    [SerializeField] private LineRenderer lineRenderer;

    private bool isActive = false;
    private float lifetime = 0f;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
    }
    private void Update() {
        UpdateLineRenderer();
    }

    private void OnEnable() {
        RotateRenderer();
        StartCoroutine(FlyCoroutine());
    }

    private void RotateRenderer() {
        float dir = transform.eulerAngles.z > 180  ?  -1  : 1;
        pivot.localScale = new Vector3(dir, 1, 1);
        pivot.localEulerAngles = new Vector3(0, 0, -transform.eulerAngles.z);
    }

    public void PullSickle() {
        StartCoroutine(PullCoroutine());
    }
    
    private IEnumerator FlyCoroutine() {
        lifetime = 0;
        isActive = true;
        isPulling = false;
        anim.clip = flyingAnimation;
        anim.Play();
        hitMonsters.Clear();
        while(lifetime < 1) {
            lifetime += Time.deltaTime / flyingTime;
            transform.Translate(Vector2.up * flyingSpeed * Time.deltaTime);
            yield return null;
        }
        isActive = false;
    }

    private IEnumerator PullCoroutine() {
        lifetime = 0;
        isPulling = true;
        isActive = true;
        anim.clip = pullingAnimation;
        anim.Play();
        hitMonsters.Clear();
        while(lifetime < 1) {
            lifetime += Time.deltaTime / pullingTime;
            Vector2 nextPos = Vector2.Lerp(transform.position, originWeapon.transform.position, lifetime*lifetime);
            transform.position = nextPos;
            yield return null;
        }
        Disapear();
    }

    private void UpdateLineRenderer() {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, originWeapon.transform.position);
    }

    private void Disapear() {
        originWeapon.EffectPooler.InPool(this.gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(!isActive)
            return;
        if(1<<other.gameObject.layer == targetLayer.value
        && !hitMonsters.Contains(other.gameObject)) {
            if(other.TryGetComponent(out Monster target)) {
                float damage;
                float hittingDelay;
                Vector2 attackForce;
                if(!isPulling) {
                    damage = originWeapon.Damage;
                    hittingDelay = flyingHittingDelay;
                    attackForce = transform.up * .6f;
                } else {
                    damage = originWeapon.PullingDamage;
                    hittingDelay = pullingHittingDelay;
                    attackForce = (originWeapon.transform.position - transform.position).normalized * 0.2f;
                }
                target.TakeDamage(damage);
                target.TakeHittingDelay(hittingDelay);
                target.TakeForce(attackForce);
                hitMonsters.Add(other.gameObject);
                GameManager.instance.Character.OnAttackMonster(target);
            }
        }
    }
}