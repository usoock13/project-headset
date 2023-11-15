using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour {
    [SerializeField] private LevelUpUI levelUpUI;
    public LevelUpUI LevelUpUI => levelUpUI;
    
    [SerializeField] private CharacterStatusUI characterUI;
    public CharacterStatusUI CharacterStatusUI => characterUI;

    private EquipmentManager _EquipmentsManager => GameManager.instance.StageManager.EquipmentsManager;

    #region Equipments UI
    [SerializeField] private List<Image> weaponsAbstracts;
    [SerializeField] private List<Image> artifactAbstracts;
    #endregion Equipments UI

    #region Visual Effect UIs
    [SerializeField] private Animation hitEffectUIAnimation;
    #endregion Visual Effect UIs
    
    public void Start() {
        UpdateWeaponList();
        UpdateArtifactList();
    }
    public void UpdateWeaponList() {
        var enumerator = _EquipmentsManager.WeaponsEnumerator.GetEnumerator();
        foreach(Image image in weaponsAbstracts) {
            if(enumerator.MoveNext()) {
                image.sprite = enumerator.Current?.Icon;
                image.color = new Color(1, 1, 1, 1.0f);
            } else {
                image.sprite = null;
                image.color = new Color(0, 0, 0, 0.3f);
            }
        }
    }
    public void UpdateArtifactList() {
        var enumerator = _EquipmentsManager.ArtifactsEnumerator.GetEnumerator();
        foreach(Image image in artifactAbstracts) {
            if(enumerator.MoveNext()) {
                image.sprite = enumerator.MoveNext() ? enumerator.Current?.Icon : null;
                image.color = new Color(1, 1, 1, 1.0f);
            } else {
                image.sprite = null;
                image.color = new Color(0, 0, 0, 0.3f);
            }
        }
    }
    public void ActiveHitEffectUI() {
        hitEffectUIAnimation.Stop();
        hitEffectUIAnimation.Play();
    }
}