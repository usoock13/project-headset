using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusUI : MonoBehaviour {
    [SerializeField] private Image characterPortrait;
    public Slider hpSlider;
    public Slider staminaSlider;
    public Slider skillGaugeSlider;
    public Slider expSlider;
    [SerializeField] private TMP_Text[] levelTexts;
    
    public void UpdateHpSlider(float ratio) {
        hpSlider.value = ratio;
    }
    public void UpdateStaminaSlider(float ratio) {
        staminaSlider.value = ratio;
    }
    public void UpdateExpSlider(float ratio) {
        expSlider.value = ratio;
    }
    public void UpdateLevel(int level) {
        foreach(TMP_Text t in levelTexts) {
            t.text = level.ToString() + " LV";
        }
    }
    public void SetPortrait(Sprite sprite) {
        characterPortrait.sprite = sprite;
    }
}