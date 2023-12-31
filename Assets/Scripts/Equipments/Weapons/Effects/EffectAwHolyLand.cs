using UnityEngine;

public class EffectAwHolyLand : MonoBehaviour {
    [SerializeField] private AweaponHolyLand originWeapon;
    private Character _Character => GameManager.instance.Character;

    private float AttackInterval => originWeapon.DamageInterval;
    private float attackCountdown = 0f;

    private float Damage => originWeapon.DamagePerSecond * originWeapon.DamageInterval;
    private float HittingDelay => originWeapon.HittingDelay;
    private float standingTime = 0;
    private float radius = 0f;
    const float MAX_STANDING_TIME = 2f;
    const float DEFAULT_RADIUS = 1f;
    const float MAX_RADIUS = 3f;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Update() {
        AttackArea();
        if(_Character.CurrentState.Compare(_Character.idleState))
            OnStandingCharacter();
        else if (_Character.CurrentState.Compare(_Character.walkState))
            OnMovingCharacter();
    }
    private void OnStandingCharacter() {
        standingTime = standingTime+Time.deltaTime>MAX_STANDING_TIME ? MAX_STANDING_TIME : standingTime+Time.deltaTime;
        UpdateAreaSize();
    }
    private void OnMovingCharacter() {
        standingTime = standingTime-Time.deltaTime<0 ? 0 : standingTime-Time.deltaTime*0.4f;
        UpdateAreaSize();
    }
    private void UpdateAreaSize() {
        float offset = standingTime / MAX_STANDING_TIME;
        radius = Mathf.Lerp(DEFAULT_RADIUS, MAX_RADIUS, offset);
        spriteRenderer.size = new Vector2(radius*2, radius*2);
    }
    private void AttackArea() {
        if(attackCountdown > 0) {
            attackCountdown -= Time.deltaTime;
        } else {
            attackCountdown = AttackInterval;
            Collider2D[] inners = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer.value);
            for(int i=0; i<inners.Length; i++) {
                if(inners[i].TryGetComponent(out Monster monster)) {

                    var attachment = originWeapon.RepentancePooler.OutPool().GetComponent<Attachment>();
                    if(monster.TryGetAttachment(attachment.AttachmentType, out var already)) {
                        monster.ReleaseAttachment(already);
                        print("already");
                    }
                    monster.TakeAttachment(attachment);

                    monster.TakeDamage(Damage);
                    if(HittingDelay > 0)
                        monster.TakeAttackDelay(HittingDelay);

                    GameManager.instance.Character.OnAttackMonster(monster);
                }
            }
        }
    }
}