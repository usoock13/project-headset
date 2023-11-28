using System;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour {
    static public GameManager instance;
    
    private StageManager stageManager;
    public StageManager StageManager {
        get { return stageManager; }
        set { stageManager ??= value; }
    }
    private UserProfileManager userProfileManager;
    public UserProfileManager UserManager => userProfileManager;
    
    public Character Character {
        get { return stageManager.Character; }
    }
    public bool GameIsOver {
        get { return stageManager.isGameOver; }
    }

    private List<Character> selectedCharacters;
    public List<Character> SelectedCharacters {
        get { return selectedCharacters; }
        set { selectedCharacters = value; }
    }

    public Action onLevelUp;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
