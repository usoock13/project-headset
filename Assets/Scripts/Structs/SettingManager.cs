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
            /* 
                TODO : Make value actually affect to game.
            */
        }
    }
    public int VolumeBGM { 
        get => setting.volumeBGM;
        set {
            setting.volumeBGM = value;
            /* 
                TODO : Make value actually affect to game.
            */
        }
    }
    public bool IsFullScreen {
        get => setting.isFullScreen;
        set {
            setting.isFullScreen = value;
            /* 
                TODO : Make value actually affect to game.
            */
        }
    }
    public int Language {
        get => setting.language;
        set {
            setting.language = value;
            /* 
                TODO : Make value actually affect to game.
            */
        }
    }

    public void ApplySetting() {
        GameManager.instance.ProfileManager.SaveGameSetting(setting);
    }
}