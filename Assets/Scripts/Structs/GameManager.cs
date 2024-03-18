using System;
using System.Collections.Generic;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour {
    static public GameManager instance;
    
    private StageManager stageManager;
    public StageManager StageManager {
        get { return stageManager; }
        set { stageManager ??= value; }
    }
    private UserProfileManager _userProfileManager;
    public UserProfileManager ProfileManager => _userProfileManager;
    
    public Character Character {
        get { return stageManager.Character; }
    }
    public CharacterInputSystem InputSystem =>
        Character.characterInputSystem;
    public bool GameIsOver {
        get { return stageManager.isGameOver; }
    }

    private List<Character> selectedCharacters;
    public List<Character> SelectedCharacters {
        get { return selectedCharacters; }
        set { selectedCharacters = value; }
    }

    [SerializeField] private EscapeMenuUI escapeMenuUI;

    public SettingManager SettingManager { get; private set; } = new SettingManager();

    public Locale SelectedLocale => LocalizationSettings.SelectedLocale;
    public string LangCodeEN => LocalizationEditorSettings.GetLocale("en").LocaleName;
    public string LangCodeKO => LocalizationEditorSettings.GetLocale("ko").LocaleName;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        if(_userProfileManager == null)
            _userProfileManager = new UserProfileManager();
    }

    public void OpenEscapeMenu() {
        if(!escapeMenuUI.gameObject.activeInHierarchy)
            escapeMenuUI.Open();
        else
            escapeMenuUI.Close();
    }
}
