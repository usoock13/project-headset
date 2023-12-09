using System.Collections;
using UnityEngine;

public class ExpJewel : Item {
    public int givingExp = 0;
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
        StartCoroutine(DropCoroutine());
        StartCoroutine(DisapearCoroutine());
    }
    public override void Pull(Transform getter) {
        if(isGround)
            base.Pull(getter);
        else
            this.getter = getter;
    }

    private IEnumerator DisapearCoroutine() {
        yield return new WaitForSeconds(120f);
        Disapear();
    }

    private IEnumerator DropCoroutine() {
        float randomAngle = Random.Range(0, 360);
        Vector2 origin = transform.position;
        Vector2 dest = Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector2.up * Random.Range(.5f, 1.5f);
        float offset = 0;
        while(offset < 1) {
            transform.position = Vector2.Lerp(origin, origin + dest, offset);
            offset += Time.deltaTime * 4f;
            yield return null;
        }
        isGround = true;
        if(getter is not null)
            this.Pull(this.getter);
    }
    public override void OnGotten() {
        onGetItem?.Invoke(this);
        GameManager.instance.StageManager.OnGetExp(this);
        GameManager.instance.Character.GetExp(givingExp);
        isGround = false;
        getter = null;
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        if(isGround)
            base.OnTriggerEnter2D(other);
    }
}