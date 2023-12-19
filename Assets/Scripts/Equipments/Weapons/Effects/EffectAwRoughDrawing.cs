using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAwRoughDrawing : MonoBehaviour {
    [SerializeField] private AWeaponMagicPen originWeapon;
    [SerializeField] private LayerMask targetLayer = 1<<8;
    
    [SerializeField] new private SpriteRenderer renderer;
    [SerializeField] private float delayTime = 0.7f;
    [SerializeField] private float effectDelay = 1.5f;
    [SerializeField] new private Animation animation;
    [SerializeField] private AnimationClip appearanceAnimation;
    [SerializeField] private AnimationClip effectAnimationClip;

    [Header("Sprtes")]
    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Sprite firstAidSprite;
    [SerializeField] private Sprite foodSprite;
    [SerializeField] private Sprite goblinSprite;
    [SerializeField] private Sprite kesoSprite;

    #region Bomb
    private float BombDamage => originWeapon.BombDamage;
    private float bombRadius = 2f;
    private float bombDelay = 1.5f;
    [SerializeField] private AnimationClip bombAnimationClip;
    [SerializeField] private ParticleSystem bombParticle;
    #endregion Bomb
    
    #region FirstAid
    #endregion FirstAid

    #region BoonFoods
    #endregion BoonFoods

    #region Goblin
    #endregion Goblin

    #region Keso
    #endregion Keso

    public enum DrawnObject {
        Bomb,
        FirstAid,
        BoonFoods,
        Goblin,
        Keso,
    }

    public void Active(DrawnObject type) {
        renderer.enabled = true;
        if(type == DrawnObject.Bomb) {
            StartCoroutine(BombCoroutine());
        } else if(type == DrawnObject.FirstAid) {
            StartCoroutine(FirstAidCoroutine());
        } else if(type == DrawnObject.BoonFoods) {
            StartCoroutine(SaladCoroutine());
        } else if(type == DrawnObject.Keso) {
            StartCoroutine(KesoCoroutine());
        } else if(type == DrawnObject.Goblin) {
            StartCoroutine(GoblinCoroutine());
        }
    }

    private IEnumerator BombCoroutine() {
        renderer.sprite = bombSprite;
        yield return new WaitForSeconds(delayTime);
        animation.clip = bombAnimationClip;
        animation.Play();
        yield return new WaitForSeconds(bombDelay);
        renderer.enabled = false;

        bombParticle.Play();
        var inners = Physics2D.OverlapCircleAll(transform.position, bombRadius, targetLayer);
        for(int i=0; i<inners.Length; i++) {
            if(inners[i].TryGetComponent(out Monster monster)) {
                monster.TakeDamage(BombDamage);
                monster.TakeAttackDelay(1.5f);
                monster.TakeForce((monster.transform.position - transform.position).normalized * 3f, 1.5f);
                GameManager.instance.Character.OnAttackMonster(monster);
            }
        }
        StartCoroutine(InPoolCoroutine());
    }

    private IEnumerator FirstAidCoroutine() {
        renderer.sprite = firstAidSprite;
        yield return new WaitForSeconds(delayTime);
        animation.clip = effectAnimationClip;
        animation.Play();
        yield return new WaitForSeconds(effectDelay);
        renderer.enabled = false;

        GameManager.instance.StageManager.CreateMeat(transform.position);
        StartCoroutine(InPoolCoroutine());
    }

    private IEnumerator SaladCoroutine() {
        renderer.sprite = foodSprite;
        yield return new WaitForSeconds(delayTime);
        animation.clip = effectAnimationClip;
        animation.Play();
        yield return new WaitForSeconds(effectDelay);
        renderer.enabled = false;
        
        GameManager.instance.StageManager.CreateSalad(transform.position);
        StartCoroutine(InPoolCoroutine());
    }

    private IEnumerator KesoCoroutine() {
        renderer.sprite = kesoSprite;
        yield return new WaitForSeconds(delayTime);
        animation.clip = effectAnimationClip;
        animation.Play();
        yield return new WaitForSeconds(effectDelay);
        renderer.enabled = false;

        GameManager.instance.StageManager.CreateKeso(transform.position, originWeapon.KesoAmount);
        GameManager.instance.StageManager.CreateKeso(transform.position, originWeapon.KesoAmount);
        GameManager.instance.StageManager.CreateKeso(transform.position, originWeapon.KesoAmount);
        StartCoroutine(InPoolCoroutine());
    }

    private IEnumerator GoblinCoroutine() {
        renderer.sprite = goblinSprite;
        yield return new WaitForSeconds(delayTime);
        animation.clip = effectAnimationClip;
        animation.Play();
        yield return new WaitForSeconds(effectDelay);
        renderer.enabled = false;

        originWeapon.GoblinPooler.OutPool(transform.position, Quaternion.identity)
            .GetComponent<EffectPaperGoblin>()?.Active(originWeapon.GoblinDamage, originWeapon.GoblinDuration);
    }

    private IEnumerator InPoolCoroutine() {
        yield return new WaitForSeconds(5f);
        animation.clip = appearanceAnimation;
        originWeapon.DrawingPooler.InPool(this.gameObject);
    }
}