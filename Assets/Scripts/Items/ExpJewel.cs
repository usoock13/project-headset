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

    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "경험의 보석";
    public override string Description => $" 강해지기 위한 경험치를 {givingExp}만큼 얻을 수 있는 보석입니다.";

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