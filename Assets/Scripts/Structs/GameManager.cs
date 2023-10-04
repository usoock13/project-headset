using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    static public GameManager instance;
    
    private StageManager stageManager;
    public StageManager StageManager {
        get { return stageManager; }
        set { stageManager ??= value; }
    }
    
    public Character Character {
        get { return stageManager.Character; }
    }
    public bool GameIsOver {
        get { return stageManager.isGameOver; }
    }

    public List<Character> selectedCharacters;

    public Action onLevelUp;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
