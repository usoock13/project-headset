using System.Collections;
using UnityEngine;

public class ExpJewel : Item {
    public int givingExp = 0;
    private Coroutine dropCoroutine;
    private bool isGround = false;
    private Transform getter = null;

    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "경험의 보석";
    public override string Description => $" 강해지기 위한 경험치를 {givingExp}만큼 얻을 수 있는 보석입니다.";

    public override void Drop() {
        base.Drop();
        dropCoroutine = StartCoroutine(DropCoroutine());
    }
    public override void PickUpItem(Transform getter) {
        if(isGround)
            base.PickUpItem(getter);
        else
            this.getter = getter;
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
            this.PickUpItem(this.getter);
    }
    public override void OnGotten() {
        base.OnGotten();
        GameManager.instance.Character.GetExp(givingExp);
        isGround = false;
        getter = null;
    }
    protected override void OnTriggerEnter2D(Collider2D other) {
        if(isGround)
            base.OnTriggerEnter2D(other);
    }
}