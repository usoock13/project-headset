using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChargingArea : MonoBehaviour {
    [SerializeField] private Slider chargingSlider;
    private float progress = 0f;
    private Coroutine currentCoroutine;
    [SerializeField] private UnityEngine.Events.UnityEvent onCharged;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer != 3)
            return;
            
        chargingSlider.gameObject.SetActive(true);
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        if(gameObject.activeSelf)
            currentCoroutine = StartCoroutine(IncreasingCoroutine());
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.layer != 3)
            return;

        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        if(gameObject.activeSelf)
            currentCoroutine = StartCoroutine(DecreasingCoroutine());
    }

    private IEnumerator IncreasingCoroutine() {
        while(progress < 1f) {
            progress += Time.deltaTime;
            chargingSlider.value = progress;
            yield return null;
        }

        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        gameObject.SetActive(false);
        onCharged?.Invoke();
    }
    private IEnumerator DecreasingCoroutine() {
        while(progress > 0f) {
            progress -= Time.deltaTime;
            chargingSlider.value = progress;
            yield return null;
        }
        progress = 0f;
        chargingSlider.gameObject.SetActive(false);
    }
}