using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour {
    [SerializeField] private LevelUpUI levelUpUI;
    public LevelUpUI LevelUpUI => levelUpUI;
    
    private Character _Character => GameManager.instance.Character;
    [SerializeField] private CharacterStatusUI characterUI;
    public CharacterStatusUI CharacterStatusUI => characterUI;

    private EquipmentManager _EquipmentsManager => GameManager.instance.StageManager.EquipmentsManager;

    #region Equipments UI
    [System.Serializable]
    private class EquipmentAbstract {
        public Image icon;
        public TMPro.TMP_Text information;
        public Slider levelSlider;
    }
    [SerializeField] private List<EquipmentAbstract> weaponsAbstracts;
    [SerializeField] private List<EquipmentAbstract> artifactAbstracts;
    #endregion Equipments UI

    #region Visual Effect UIs
    [SerializeField] private Animation hitEffectUIAnimation;
    #endregion Visual Effect UIs
    
    public void InitializeStatusUI() {
        UpdateWeaponList();
        UpdateArtifactList();
        characterUI.hpSlider = _Character.hpSlider;
        characterUI.staminaSlider = _Character.staminaSlider;
        characterUI.UpdateExpSlider(0);
        characterUI.UpdateHpSlider(_Character.currentHp / _Character.MaxHp);
        characterUI.UpdateLevel(_Character.level);
    }
    public void UpdateWeaponList() {
        var enumerator = _EquipmentsManager.WeaponsEnumerator.GetEnumerator();
        foreach(EquipmentAbstract item in weaponsAbstracts) {
            if(enumerator.MoveNext()) {
                item.icon.sprite = enumerator.Current?.Icon;
                item.icon.color = new Color(1, 1, 1, 1.0f);
                item.information.text = enumerator.Current.extraInformation;
                item.levelSlider.value = enumerator.Current.CurrentLevel;
            } else {
                item.icon.sprite = null;
                item.icon.color = new Color(0, 0, 0, 0f);
                item.information.text = "";
                item.levelSlider.value = 0;
            }
        }
    }
    public void UpdateArtifactList() {
        var enumerator = _EquipmentsManager.ArtifactsEnumerator.GetEnumerator();
        foreach(EquipmentAbstract item in artifactAbstracts) {
            if(enumerator.MoveNext()) {
                item.icon.sprite = enumerator.Current?.Icon;
                item.icon.color = new Color(1, 1, 1, 1.0f);
                item.information.text = enumerator.Current.extraInformation;
                item.levelSlider.value = enumerator.Current.CurrentLevel;
            } else {
                item.icon.sprite = null;
                item.icon.color = new Color(0, 0, 0, 0f);
                item.information.text = "";
                item.levelSlider.value = 0;
            }
        }
    }
    public void ActiveHitEffectUI() {
        hitEffectUIAnimation.Stop();
        hitEffectUIAnimation.Play();
    }
}