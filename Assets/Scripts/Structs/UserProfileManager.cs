using System.IO;
using UnityEngine;

public class UserProfileManager {
    private UserProfile profile;

    private string saveFileName = "userProfile.json";

    public int HavingKeso => profile.currentKeso;
    public bool[] UnlockCharacterList => profile.unlockRharacterList;

    public UserProfileManager() {
        LoadFromLocal();
    }
    public void IncreaseKeso(int amount) {
        try {
            checked {
                profile.currentKeso += amount;
            }
        } catch {
            profile.currentKeso = int.MaxValue;
        }
        SaveToLocal();
    }

    private void LoadFromLocal() {
        string json = FileManager.LoadJson(saveFileName);
        if(json == null || json == "")
            profile = new UserProfile();
        else
            profile = JsonUtility.FromJson<UserProfile>(json);
    }

    private void SaveToLocal() {
        string json = JsonUtility.ToJson(profile);
        FileManager.SaveJson(json, saveFileName);
    }

    public void SaveGameSetting(SettingManager.GameSetting setting) {
        string json = JsonUtility.ToJson(setting);
        PlayerPrefs.SetString("setting", json);
    }

    public string LoadGameSetting() {
        return PlayerPrefs.GetString("setting") ?? null;
    }

    private class UserProfile {
        public int currentKeso;
        public bool[] unlockRharacterList;
        public UserProfile() {
            this.currentKeso = 0;
            this.unlockRharacterList = new bool[8];
        }
    }
}