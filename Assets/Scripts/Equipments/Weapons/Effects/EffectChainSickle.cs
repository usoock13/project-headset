using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChainSickle : MonoBehaviour {
    public WeaponChainSickle originWeapon;

    private float flyingSpeed = 24f;
    private float currentFlyingSpeed = 24f;
    private float flyingTime = 0.3f;
    private float flyingHittingDelay = 0.5f;

    private bool isPulling = false;
    private float pullingTime = 0.8f;
    private float pullingHittingDelay = 0.4f;

    private List<GameObject> hitMonsters = new List<GameObject>();
    [SerializeField] private LayerMask targetLayer = 1<<8;
    [SerializeField] private Transform pivot;
    [SerializeField] private Animation anim;
    [SerializeField] private AnimationClip flyingAnimation;
    [SerializeField] private AnimationClip pullingAnimation;

    [SerializeField] private SpriteRenderer sickleRenderer;
    [SerializeField] private Sprite flyingSprite;
    [SerializeField] private Sprite hookedSprite;

    [SerializeField] private Transform chainStartingPoint;
    [SerializeField] private LineRenderer lineRenderer;
    private int chainStep = 20;
    private float chainYAddition = 1f;

    private bool isActive = false;
    private float lifetime = 0f;

    private Coroutine flyingCoroutine;

    private void Start() {
        if(originWeapon == null) {
            Debug.LogError($"{this.gameObject.name}'s 'origin weapon' variable is not definded.\nThis game object will be destoyed.");
            Destroy(gameObject);
        }
        lineRenderer.positionCount = chainStep+1;
    }
    private void Update() {
        UpdateLineRenderer();
    }

    private void OnEnable() {
        RotateRenderer();
        flyingCoroutine = StartCoroutine(FlyCoroutine());
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
        currentFlyingSpeed = flyingSpeed;
        anim.clip = flyingAnimation;
        anim.Play();
        hitMonsters.Clear();
        while(lifetime < 1) {
            lifetime += Time.deltaTime / flyingTime;
            transform.Translate(Vector2.up * currentFlyingSpeed * Time.deltaTime);
            chainYAddition = lifetime;
            yield return null;
        }
        HookSickle();
    }
    private void HookSickle() {
        if(flyingCoroutine != null)
            StopCoroutine(flyingCoroutine);
        sickleRenderer.sprite = hookedSprite;
        isActive = false;
    }

    private IEnumerator PullCoroutine() {
        lifetime = 0;
        isPulling = true;
        isActive = true;
        anim.clip = pullingAnimation;
        anim.Play();
        hitMonsters.Clear();
        sickleRenderer.sprite = flyingSprite;
        Vector3 origin = transform.position;
        while(lifetime < 1) {
            lifetime += Time.deltaTime / pullingTime;
            Vector2 nextPos = Vector2.Lerp(origin, originWeapon.transform.position, Mathf.Pow(lifetime, 5));
            transform.position = nextPos;
            yield return null;
            chainYAddition = 1-lifetime;
        }
        Disappear();
    }

    private void UpdateLineRenderer() {
        Vector3 next;
        float yy;
        for(int i=0; i<=chainStep; i++) {
            yy = Mathf.Lerp(-1, 1, (float)i/chainStep);
            yy = 1-Mathf.Pow(yy, 2);
            next = Vector3.Lerp(transform.position, originWeapon.transform.position+Vector3.up*0.3f, (float)i / chainStep) - (Vector3.up * yy * chainYAddition);
            lineRenderer.SetPosition(i, next);
        }
    }

    private void Disappear() {
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
                    currentFlyingSpeed *= 0.7f;
                } else {
                    damage = originWeapon.PullingDamage;
                    hittingDelay = pullingHittingDelay;
                    attackForce = (originWeapon.transform.position - transform.position).normalized * .5f;
                }
                target.TakeDamage(damage);
                target.TakeAttackDelay(hittingDelay);
                target.TakeForce(attackForce);
                hitMonsters.Add(other.gameObject);
                GameManager.instance.Character.OnAttackMonster(target);
            }
        }
    }
}