using UnityEngine;

public class Movement : MonoBehaviour {
    int blockLayer = 6;

    public void MoveToward(Vector2 moveVector) {
        transform.Translate(moveVector);
    }
}