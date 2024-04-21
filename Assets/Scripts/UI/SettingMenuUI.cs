using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuUI : MonoBehaviour {
    private SettingManager SettingManager => GameManager.instance.SettingManager;

    [Header("UIs")]
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Slider effectSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Button languageButton;
    [SerializeField] private TMP_Text languageButtonText;

    private int currentLanguage = 0;
    private string[] languageList = new string[] { "English", "한국어" };

    public Action<byte> onChangeLanguage;

    private SettingManager.GameSetting prevSetting;

    public void Open() {
        gameObject.SetActive(true);
        InitializeUI();
        prevSetting = new SettingManager.GameSetting() {
            isFullScreen = SettingManager.IsFullScreen,
            volumeEffect = SettingManager.VolumeEffect,
            volumeBGM = SettingManager.VolumeBGM,
            language = SettingManager.Language,
        };
        BringSettingToUI();
    }

    private void InitializeUI() {
        fullScreenToggle.isOn = SettingManager.IsFullScreen;
        effectSlider.value = SettingManager.VolumeEffect;
        bgmSlider.value = SettingManager.VolumeBGM;
        languageButtonText.text = languageList[SettingManager.Language];
    }

    #region UI Events
    public void OnChangeFullScreen() {
        SettingManager.IsFullScreen = fullScreenToggle.isOn;
    }
    public void OnChangeVolumeEffect() {
        SettingManager.VolumeEffect = (int)effectSlider.value;
    }
    public void OnChangeVolumeBGM() {
        SettingManager.VolumeBGM = (int) bgmSlider.value;
    }
    public void OnChangeLanguage() {
        currentLanguage ++;
        currentLanguage %= languageList.Length;
        languageButtonText.text = languageList[currentLanguage];

        SettingManager.Language = currentLanguage;
    }
    
    public void BringSettingToUI() {
        // SettingManager.LoadSetting();
        fullScreenToggle.isOn = SettingManager.IsFullScreen;
        effectSlider.value = SettingManager.VolumeEffect;
        bgmSlider.value = SettingManager.VolumeBGM;
        currentLanguage = SettingManager.Language;
        languageButtonText.text = languageList[currentLanguage];
    }

    public void OnClickApplyButton() {
        SettingManager.ApplySetting();
        this.gameObject.SetActive(false);
    }

    public void OnClickCancelButton() {
        SettingManager.IsFullScreen = prevSetting.isFullScreen;
        SettingManager.VolumeEffect = prevSetting.volumeEffect;
        SettingManager.VolumeBGM = prevSetting.volumeBGM;
        SettingManager.Language = prevSetting.language;
        this.gameObject.SetActive(false);
    }
    #endregion UI Events
}