using System.Collections;
using UnityEngine;

public class ExpJewel : Item {
    private int givingExp = 0;
    public int GivingExp {
        get => givingExp;
        set {
            givingExp = value;
            if(givingExp > SPLIT_SPRITE_BORDER_2) {
                spriteRenderer.sprite = spriteList[2];
                return;
            }
            if(givingExp > SPLIT_SPRITE_BORDER_1) {
                spriteRenderer.sprite = spriteList[1];
                return;
            }
            spriteRenderer.sprite = spriteList[0];
        }
    }
    
    private bool isGround = false;
    private Transform getter = null;

    [SerializeField] private Sprite _icon;

    protected override ItemInformation InformationEN => new ItemInformation(
        Icon: _icon,
        Name: "Experience Jewel",
        Description:
            $"The jewel to get <color=#f40>{givingExp} EXP</color>/"
    );
    protected override ItemInformation InformationKO => new ItemInformation(
        Icon: _icon,
        Name: "경험의 보석",
        Description:
            $"<color=#f40>{givingExp}의 경험치</color>를 제공하는 보석입니다."
    );

    private const int SPLIT_SPRITE_BORDER_1 = 50;
    private const int SPLIT_SPRITE_BORDER_2 = 200;

    [SerializeField] private Sprite[] spriteList;

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
        GameManager.instance.Character.GetExp(givingExp);
    }
}