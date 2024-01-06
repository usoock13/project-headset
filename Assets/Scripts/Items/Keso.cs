using System.Collections;
using UnityEngine;

public class Keso : Item {
    [SerializeField] private Sprite _icon;

    protected override ItemInformation InformationEN => new ItemInformation(
        Icon: _icon,
        Name: "Keso",
        Description:
            "<nobr>어디에서나 균등한 가치를 보장 받을 수 있는 대륙 공용 화폐입니다. 캐릭터를 고용하고 훈련시키기 위해 사용할 수 있습니다.</nobr>"
    );
    protected override ItemInformation InformationKO => new ItemInformation(
        Icon: _icon,
        Name: "케소",
        Description:
            "<nobr>어디에서나 균등한 가치를 보장 받을 수 있는 대륙 공용 화폐입니다. 캐릭터를 고용하고 훈련시키기 위해 사용할 수 있습니다.</nobr>"
    );

    [SerializeField] private Sprite[] spriteList;
    
    private const int SPLIT_SPRITE_BORDER_1 = 20;
    private const int SPLIT_SPRITE_BORDER_2 = 50;

    private int amount = 0;
    public int Amount {
        get => amount;
        set {
            amount = value;
            if(amount > SPLIT_SPRITE_BORDER_2) {
                spriteRenderer.sprite = spriteList[2];
                return;
            }
            if(amount > SPLIT_SPRITE_BORDER_1) {
                spriteRenderer.sprite = spriteList[1];
                return;
            }
            spriteRenderer.sprite = spriteList[0];
        }
    }

    public override void Drop() {
        base.Drop();
        StartCoroutine(DisappearCoroutine());
    }

    private IEnumerator DisappearCoroutine() {
        yield return new WaitForSeconds(120f);
        Disappear();
    }

    public override void OnGotten() {
        base.OnGotten();
        GameManager.instance.StageManager.IncreaseKesoEarned(amount);
    }
    
}