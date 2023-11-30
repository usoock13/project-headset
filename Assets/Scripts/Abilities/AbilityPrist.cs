using System;
using UnityEngine;

public class AbilityPrist : Ability {
    private float timeToHeal = 2f;
    private float standingTime = 0;
    private float cooldown = 0;
    [SerializeField] private float healAmount = 3f;
    [SerializeField] private ParticleSystem healParticle;

    private Character owner;

    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "기도";
    public override string Description => $"<nbr>이동하지 않는 2초마다 체력을 회복합니다.</nbr>";

    public override void OnTaken(Character character) {
        owner = character;
        healParticle.transform.position = (Vector2)owner.transform.position - Vector2.up*.1f;
        character.extraRecoveringSp += GetExtraRecoveringSP;
    }
    public override void OnReleased(Character character) {
        character.extraRecoveringSp -= GetExtraRecoveringSP;
    }
    private void Update() {
        if(owner.CurrentState.Compare(owner.idleState)) {
            standingTime = standingTime+Time.deltaTime > timeToHeal   ?   timeToHeal   :   standingTime + Time.deltaTime;
            if(standingTime >= timeToHeal)
                HealCharacter();
        } else {
            standingTime = 0;
        }
    }
    private void HealCharacter() {
        cooldown += Time.deltaTime;
        if(cooldown >= 2) {
            cooldown -= 2;
            owner.TakeHeal(healAmount);
            healParticle.Play();
        }
    }
    private float GetExtraRecoveringSP (Character character) => 0.5f;
}