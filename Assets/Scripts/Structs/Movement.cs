using System;
using UnityEngine;

public class Movement : MonoBehaviour {
    [SerializeField] private float radius = 1;
    private const int BLOCK_LAYER = 7;

    public void Start() {
        radius = GetComponent<CircleCollider2D>()?.radius ?? radius;
    }
    public void MoveToward(Vector2 moveVector) {
        if(Physics2D.OverlapCircle(transform.position, radius, 1<<BLOCK_LAYER))
            return;

        float angle = Vector3.Angle(Vector3.up, moveVector);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius*1.1f, moveVector, moveVector.magnitude, 1<<BLOCK_LAYER);
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
        transform.Translate(moveVector);
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer == BLOCK_LAYER) {
            transform.Translate(((Vector2)transform.position - other.ClosestPoint(transform.position)) * Time.deltaTime);
            print("Character in the collider that has block layer");
        }
    }
}