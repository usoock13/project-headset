using System.Collections;
using UnityEngine;

public class ExpJewel : Item {
    public float givingExp = 0;

    public override void Drop() {
        base.Drop();
        gameObject.SetActive(true);
        StartCoroutine(DropCoroutine());
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
}