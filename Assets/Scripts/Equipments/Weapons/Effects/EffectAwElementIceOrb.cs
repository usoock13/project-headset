using UnityEngine;

public class EffectAwElementIceOrb : EffectAwElement
{
    protected override void AttackMonsters(Monster[] monsters) {
        for(int i=0; i<monsters.Length; i++) {
            Vector3 characterPoint = GameManager.instance.Character.transform.position;
            Vector2 forceDir = (monsters[i].transform.position - characterPoint).normalized;
            AttachToMonster(monsters[i]);
            monsters[i].TakeDamage(originWeapon.Damage);
            monsters[i].TakeAttackDelay(hittingDelay);
            monsters[i].TakeForce(forceDir * attackForce, hittingDelay);
            GameManager.instance.Character.OnAttackMonster(monsters[i]);
        }
    }

    private void AttachToMonster(Monster monster) {
        var attachment = originWeapon.FreezePooler.OutPool().GetComponent<AttachmentFreeze>();
        if(monster.TryGetAttachment(attachment.AttachmentType, out var already))
            monster.ReleaseAttachment(already);
        attachment.duration = originWeapon.FreezeDuration;
        monster.TakeAttachment(attachment);
    }
}