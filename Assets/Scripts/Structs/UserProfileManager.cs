using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class UserProfileManager {
    public  UserProfile Profile { get; private set; }

    private string saveDir = Application.persistentDataPath + "\\headset\\Save Data\\";
    private string saveFileName = "userProfile.json";

    public UserProfileManager() {
        LoadFromLocal();
    }
    public void IncreaseKeso(int amount) {
        try {
            checked {
                Profile.currentKeso += amount;
            }
        } catch {
            Profile.currentKeso = int.MaxValue;
        }
        SaveToLocal();
    }
    private void LoadFromLocal() {
        string path = saveDir + saveFileName;
        if(Directory.Exists(saveDir) && File.Exists(path)) {
            var sr = new StreamReader(path);
            string json = sr.ReadToEnd();
            Profile = JsonUtility.FromJson<UserProfile>(json);
        } else {
            CreateNewSaveData();
        }
    }
    private void SaveToLocal() {
        if(!Directory.Exists(saveDir) && !File.Exists(saveDir + saveFileName)) {
            CreateNewSaveData();
        }
        StreamWriter sw = new StreamWriter(saveDir + saveFileName);
        string json = JsonUtility.ToJson(Profile);
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

    public class UserProfile {
        public int currentKeso;
        public bool[] characterUnlockedList;
        public UserProfile() {
            this.currentKeso = 0;
            this.characterUnlockedList = new bool[8];
        }
    }
}