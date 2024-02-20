using System.Collections;
using UnityEngine;

public class Keso : Item {
    [SerializeField] private Sprite _icon;
    
    protected override ItemInformation InformationEN => new ItemInformation(
        Icon: _icon,
        Name: "Keso",
        Description:
            "<nobr>The common currency. It can be used for a variety of uses.</nobr>"
    );
    protected override ItemInformation InformationKO => new ItemInformation(
        Icon: _icon,
        Name: "케소",
        Description:
            "<nobr>공용 화폐입니다. 다양한 사용처에 활용될 수 있습니다.</nobr>"
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