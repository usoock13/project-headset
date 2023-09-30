using System.Collections;
using UnityEngine;

public class ExpJewel : Item {
    public int givingExp = 0;
    private Coroutine dropCoroutine;

    [SerializeField] private Sprite icon;
    public override Sprite Icon => icon;
    public override string Name => "경험의 보석";
    public override string Description => $" 강해지기 위한 경험치를 {givingExp}만큼 얻을 수 있는 보석입니다.";

    public override void Drop() {
        base.Drop();
        gameObject.SetActive(true);
        dropCoroutine = StartCoroutine(DropCoroutine());
    }
    public override void PickUpItem(Transform getter) {
        base.PickUpItem(getter);
        if(dropCoroutine != null)
            StopCoroutine(dropCoroutine);
    }
    private IEnumerator DropCoroutine() {
        float randomAngle = Random.Range(0, 360);
        Vector2 origin = transform.position;
        Vector2 dest = Quaternion.AngleAxis(randomAngle, Vector3.forward) * Vector2.up * Random.Range(.5f, 1.5f);
        float offset = 0;
        while(offset < 1) {
            transform.position = Vector2.Lerp(transform.position, origin + dest, offset);
            offset += Time.deltaTime;
            yield return null;
        }
    }
    public override void OnGotten() {
        base.OnGotten();
        GameManager.instance.Character.GetExp(givingExp);
    }
}