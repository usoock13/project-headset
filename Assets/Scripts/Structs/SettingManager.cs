using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Utility;

public class SettingManager {
    private GameSetting setting;
    
    public SettingManager() {
        setting = new GameSetting() {
            isFullScreen = true,
            volumeEffect = 50,
            volumeBGM = 50,
            language = 0,
        };
    }

    public struct GameSetting {
        public bool isFullScreen;
        public int volumeEffect;
        public int volumeBGM;
        public int language;
    }

    public int VolumeEffect { 
        get => setting.volumeEffect;
        set {
            setting.volumeEffect = value;
            GameManager.instance.SoundManager.VolumeEffect = value * 0.01f;
        }
    }
    public int VolumeBGM { 
        get => setting.volumeBGM;
        set {
            setting.volumeBGM = value;
            GameManager.instance.SoundManager.VolumeBGM = value * 0.01f;
        }
    }
    public bool IsFullScreen {
        get => setting.isFullScreen;
        set {
            setting.isFullScreen = value;
            Screen.fullScreen = value;
            /* 
                TODO : Make value actually affect to game.
            */
        }
    }
    public int Language {
        get => setting.language;
        set {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value];
            setting.language = value;
        }
    }

    public void LoadSetting() {
        string json = GameManager.instance.ProfileManager.LoadGameSetting();
        if(json == null)
            this.setting = new GameSetting();
        else
            this.setting = JsonUtility.FromJson<GameSetting>(json);
    }

    public IEnumerator SynchronizeSetting() {
        yield return new WaitForCondition(() => LocalizationSettings.AvailableLocales.Locales.Count > 0);

        this.IsFullScreen = setting.isFullScreen;
        this.VolumeEffect = setting.volumeEffect;
        this.VolumeBGM = setting.volumeBGM;
        this.Language = setting.language;
    }

    public void ApplySetting() {
        GameManager.instance.ProfileManager.SaveGameSetting(setting);
    }
}