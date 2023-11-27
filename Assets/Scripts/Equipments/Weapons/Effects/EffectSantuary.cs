using UnityEngine;

public class EffectSantuary : MonoBehaviour {
    [SerializeField] private WeaponSantuary originWeapon;
    private Character _Character => GameManager.instance.Character;

    const float ATTACK_INTERVAL = 0.5f;
    const float HEAL_INTERVAL = 2f;
    private float attackCountdown = 0f;

    private float Damage => originWeapon.Damage;
    private float HittingDelay => originWeapon.HittingDelay;
    [SerializeField] private float radius = 2f;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Update() {
        AttackArea();
    }
    private void AttackArea() {
        if(attackCountdown > 0) {
            attackCountdown -= Time.deltaTime;
        } else {
            attackCountdown = ATTACK_INTERVAL;
            Collider2D[] inners = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer.value);
            for(int i=0; i<inners.Length; i++) {
                if(inners[i].TryGetComponent(out Monster target)) {
                    target.TakeDamage(Damage);
                    if(HittingDelay > 0)
                        target.TakeHittingDelay(HittingDelay);
                }
            }
        }
    }
}