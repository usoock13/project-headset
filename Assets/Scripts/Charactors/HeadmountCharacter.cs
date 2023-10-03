using UnityEngine;

public class HeadmountCharacter : MonoBehaviour {
    public Transform headmountPoint;

    [SerializeField] private SpriteRenderer handsSprite;
    [SerializeField] private SpriteRenderer frontSprite;
    [SerializeField] private SpriteRenderer backSprite;
    public SpriteRenderer HandsSprite => handsSprite;
    public SpriteRenderer FrontSprite => frontSprite;
    public SpriteRenderer BackSprite => backSprite;
}