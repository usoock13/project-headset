using UnityEngine;

public class HeadmountCharacter : MonoBehaviour {
    public Transform headmountPoint;
    [SerializeField] private Transform anchor;

    public void SetRenderPointAnchor(Transform anchor, int order) {
        this.anchor = anchor;
    }

    [SerializeField] private SpriteRenderer handsSprite;
    [SerializeField] private SpriteRenderer frontSprite;
    [SerializeField] private SpriteRenderer backSprite;
    public SpriteRenderer HandsSprite => handsSprite;
    public SpriteRenderer FrontSprite => frontSprite;
    public SpriteRenderer BackSprite => backSprite; 
}