using System;
using System.IO;
using UnityEngine;

public class UserProfileManager {
    private UserProfile profile;

    private string saveDir = Application.persistentDataPath + "\\headset\\Save Data\\";
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
        string path = saveDir + saveFileName;
        if(Directory.Exists(saveDir) && File.Exists(path)) {
            var sr = new StreamReader(path);
            string json = sr.ReadToEnd();
            profile = JsonUtility.FromJson<UserProfile>(json);
        } else {
            CreateNewSaveData();
        }
    }
    private void SaveToLocal() {
        if(!Directory.Exists(saveDir) && !File.Exists(saveDir + saveFileName)) {
            CreateNewSaveData();
        }
        StreamWriter sw = new StreamWriter(saveDir + saveFileName);
        string json = JsonUtility.ToJson(profile);
        sw.WriteLine(json);
        sw.Close();
    }
    private void CreateNewSaveData() {
        if(!Directory.Exists(saveDir))
            Directory.CreateDirectory(saveDir);
        StreamWriter sw = new StreamWriter(saveDir + saveFileName);
        string json = JsonUtility.ToJson(new UserProfile());
        sw.WriteLine(json);
        sw.Close();
    }

    public void SaveGameSetting(SettingManager.GameSetting setting) {
        string json = JsonUtility.ToJson(setting);
        Debug.Log(json);
        /* 
            TODO : save as file. (setting.json)
         */
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