using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusUI : MonoBehaviour {
    [SerializeField] private Slider expSlider;
    
    public void Start() {
        expSlider.value = 0;
    }
    public void UpdateExpSlider(float ratio) {
        expSlider.value = ratio;
    }
}