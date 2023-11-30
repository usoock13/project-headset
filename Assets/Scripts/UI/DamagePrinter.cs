using System.Collections;
using UnityEngine;

public class DamagePrinter : MonoBehaviour {
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject damageNumber;
    private ObjectPooler numbersPooler;
    
    private void Start() {
        numbersPooler = new ObjectPooler(damageNumber, null, (number) => { number.transform.SetParent(canvas); }, parent: canvas.transform);
    }
    public void PrintDamage(Vector2 point, string number, Color color) {
        GameObject instance = numbersPooler.OutPool(point, Quaternion.identity);
        TMPro.TMP_Text tmp = instance.GetComponentInChildren<TMPro.TMP_Text>();
        tmp.text = number;
        tmp.color = color;
        StartCoroutine( InPoolCoroutine(instance, 5) );
    }
    private IEnumerator InPoolCoroutine(GameObject target, float second) {
        yield return new WaitForSeconds(second);
        numbersPooler.InPool(target);
    }
}
