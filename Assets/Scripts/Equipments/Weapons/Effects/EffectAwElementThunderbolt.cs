using UnityEngine;

public class EffectAwElementThunderbolt : EffectAwElement
{
    protected override void AttackMonsters(Monster[] monsters) {
        for(int i=0; i<monsters.Length; i++) {
            Vector3 characterPoint = GameManager.instance.Character.transform.position;
            Vector2 forceDir = (monsters[i].transform.position - characterPoint).normalized;
            monsters[i].TakeDamage(originWeapon.Damage);
            monsters[i].TakeAttackDelay(hittingDelay);
            monsters[i].TakeForce(forceDir * attackForce, hittingDelay);
            GameManager.instance.Character.OnAttackMonster(monsters[i]);
            AttachToMonster(monsters[i]);
        }
    }

    private void AttachToMonster(Monster monster) {
        var attachment = originWeapon.ShockPooler.OutPool().GetComponent<AttachmentShock>();
        if(monster.TryGetAttachment(attachment.AttachmentType, out var already))
            monster.ReleaseAttachment(already);
        attachment.damagePerSecond = originWeapon.ShockDamage;
        attachment.duration = originWeapon.ShockDuration;
        monster.TakeAttachment(attachment);
    }
}