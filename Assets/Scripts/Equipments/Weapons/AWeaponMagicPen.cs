using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AWeaponMagicPen : Weapon {
    [SerializeField] private EffectMagicDrawing effect;

    #region Weapon Status
    private const int MAX_LEVEL = 5;
    public override int MaxLevel => MAX_LEVEL;

    public float Interval => AttackInterval;
    protected override float AttackInterval => 0.12f;
    #endregion Weapon Status

    #region Weapon Information
    [SerializeField] private Sprite _weapnoIcon;
    public override Sprite Icon => _weapnoIcon;
    public override string Name => "마술 만연필";
    public override string Description => 
        $"<nobr>"
      + $""
      + "</nobr";
    #endregion Weapon Information

    protected override void Attack() {
        float angle = UnityEngine.Random.Range(-22f, 22f);
        effect.transform.rotation = _Character.attackArrow.rotation;
        effect.transform.Rotate(Vector3.forward, angle);
        effect.AttackForward();
        _Character.OnAttack();
    }
}
