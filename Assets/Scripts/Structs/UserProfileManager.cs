using System;
using System.IO;
using UnityEngine;

public class UserProfileManager {
    private UserProfile _profile;

    private string saveDir = Application.persistentDataPath + "\\headset\\Save Data";
    private string saveFileName = "userProfile.json";
    
    public void IncreaseKeso(int amount) {
        try {
            checked {
                _profile.currentKeso += amount;
            }
        } catch {
            _profile.currentKeso = int.MaxValue;
        }
    }

    private void SaveToLocal() { 
        string json = JsonUtility.ToJson(_profile);
        FileStream fs = new FileInfo(saveDir + saveFileName).Create();
    }

    public class UserProfile {
        public int currentKeso = 0;
        
    }
}