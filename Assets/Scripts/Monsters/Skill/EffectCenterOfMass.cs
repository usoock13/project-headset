using UnityEngine;

public class EffectCenterOfMass : MonoBehaviour {
    private Character target = null;
    [SerializeField] private CircleCollider2D border;
    private float BorderRadius => border.radius;
    private bool isActive = false;

    private float pullingForce = 2f;

    public void Active() {
        target = GameManager.instance.StageManager.Character;
        isActive = true;
    }

    public void Inactive() {
        isActive = false;
    }

    private void Update() {
        if(isActive) {
            float dist = Vector2.Distance(this.transform.position, target.transform.position);
            dist -= BorderRadius;
            if(dist > 0) {
                Vector2 dir = (this.transform.position - target.transform.position).normalized;
                /* temporary >> */
                target.transform.Translate((pullingForce + dist) * dir * Time.deltaTime, Space.World);
                /* <<  */
            }
        }
    }
}