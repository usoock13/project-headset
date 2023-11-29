using System.Collections;
using UnityEngine;

public class ItemKeso : Item {
    [SerializeField] private Sprite _icon;
    public override Sprite Icon => _icon;
    public override string Name => "케소";
    public override string Description => "<nobr>어디에서나 균등한 가치를 보장 받을 수 있는 대륙 공용 화폐입니다. 캐릭터를 고용하고 훈련시키기 위해 사용할 수 있습니다.</nobr>";

    public int amount = 0;
    public override void OnGotten() {
        base.OnGotten();
        GameManager.instance.UserManager.IncreaseKeso(amount);
    }
}