using System;
using UnityEngine;

public class Movement : MonoBehaviour {
    private float bounds;
    [SerializeField] private LayerMask blockLayer = 1<<7;

    public void Start() {
        var size = GetComponent<Collider2D>().bounds.size;
        bounds = Mathf.Min(size.x, size.y);
    }
    public void MoveToward(Vector2 moveVector) {
        // if(Physics2D.OverlapCircle(transform.position, radius, blockLayer.value))
        //     return;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, bounds*1.1f, moveVector, moveVector.magnitude, blockLayer.value);
        Vector2 normal = moveVector.normalized;
        foreach(RaycastHit2D hit in hits) {
            if(hit.transform.gameObject != gameObject) {
                float xx = normal.x + hit.normal.x;
                float yy = normal.y + hit.normal.y;
                normal = new Vector3(xx, yy);
            }
        }
        Debug.DrawLine(transform.position + (Vector3)normal, transform.position + (Vector3)normal*5, Color.red);
        Vector2 final = normal * moveVector.magnitude;
        transform.Translate(final);
    }
    public void Translate(Vector2 moveVector) {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.5f, moveVector, moveVector.magnitude, blockLayer.value);
        int count = 0;
        for(int i=0; i<hits.Length; i++) {
            if(hits[i].transform.gameObject != this.gameObject) {
                transform.Translate((transform.position - hits[i].transform.position) * 1f * Time.deltaTime);
                if(++ count > 3)
                    break;
            }
        }
        transform.Translate(moveVector);
    }
    private void OnTriggerStay2D(Collider2D other) {
        if((1<<other.gameObject.layer & blockLayer.value) > 0
        && other.gameObject != this.gameObject) {
            transform.Translate(((Vector2)transform.position - other.ClosestPoint(transform.position)) * Time.deltaTime);
            print($"{gameObject.name} is in the collider that has block layer");
        }
    }
}