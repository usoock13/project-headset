using System.Diagnostics;
using UnityEngine;

public class __Test : MonoBehaviour {
    [SerializeField] LineRenderer line;

    [SerializeField] Transform a;
    [SerializeField] Transform b;
    [SerializeField] int step = 20;
    
    private void Update() {
        Vector3 next;
        float yy;
        for(int i=0; i<=step; i++) {
            yy = Mathf.Lerp(-1, 1, (float)i/step);
            yy = 1-Mathf.Pow(yy, 2);
            next = Vector3.Lerp(a.position, b.position, (float)i / step) - Vector3.up * yy;
            line.SetPosition(i, next);
        }
    }
}
