using System;
using System.Xml.Schema;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

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
                // float xx = Mathf.Abs(normal.x)>Mathf.Abs(hit.normal.x) ? normal.x+hit.normal.x : 0;
                // float yy = Mathf.Abs(normal.y)>Mathf.Abs(hit.normal.y) ? normal.y+hit.normal.y : 0;
                // print(Mathf.Atan2(normal.x*hit.normal.y - normal.y*hit.normal.x, normal.x*hit.normal.x + normal.y*hit.normal.y)*Mathf.Rad2Deg);
                float xx = normal.x + hit.normal.x;
                float yy = normal.y + hit.normal.y;
                
                // float ag = Mathf.Atan2(normal.x*hit.normal.y - normal.y*hit.normal.x, normal.x*hit.normal.x + normal.y*hit.normal.y);
                // Vector2 next = new Vector2(xx, yy).magnitude * (Quaternion.AngleAxis(ag<0 ? 90 : -90, Vector3.forward) * hit.normal);
                // normal = next;
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
            print((Vector2)transform.position - other.ClosestPoint(transform.position));
        }
    }
}