using UnityEngine;

public class HeadmountCharacter : MonoBehaviour {
    public Transform headmountPoint;
    [SerializeField] private Transform anchor;

    public void SetRenderPointAnchor(Transform anchor, int order) {
        this.anchor = anchor;
        // transform.position -= anchor.localPosition;
        // headmountPoint.position += anchor.localPosition;
    }
    private void Update() {
        // handsSprite.material.SetVector("_Additional_Point", anchor.transform.localPosition);
        // frontSprite.material.SetVector("_Additional_Point", anchor.transform.localPosition);
        // backSprite.material.SetVector("_Additional_Point", anchor.transform.localPosition);
    }

    [SerializeField] private SpriteRenderer handsSprite;
    [SerializeField] private SpriteRenderer frontSprite;
    [SerializeField] private SpriteRenderer backSprite;
    public SpriteRenderer HandsSprite => handsSprite;
    public SpriteRenderer FrontSprite => frontSprite;
    public SpriteRenderer BackSprite => backSprite; 
}