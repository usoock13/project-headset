using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    [SerializeField] private CharacterSelectUI characterSelectUI;
    [SerializeField] private GameObject introUIGobj;

    [SerializeField] private TMP_Text kesoAmount;

    [SerializeField] private AudioClip clickIntroClip;
    [SerializeField] private AudioClip clickButtonClip;

    [SerializeField] private AudioClip lobbyBgmClip;

    private SoundManager SoundManager => GameManager.instance.SoundManager;

    private void Start() {
        InitializeUIs();
        SoundManager.PlayBGM(lobbyBgmClip);
    }

    private void InitializeUIs() {
        UpdateKesoAmount(GameManager.instance.ProfileManager.HavingKeso);
    }

    public void OnClickIntroUI() {
        ShowIntroUI(false);
        ShowCharacterSelectUI(true);
        SoundManager.PlayEffect(clickIntroClip);
    }

    public void OnClickGameStartButton() {
        SceneManager.LoadScene(1);
    }
    private void ShowCharacterSelectUI(bool visibility) {
        characterSelectUI.gameObject.SetActive(visibility);
    }
    private void ShowIntroUI(bool visibility) {
        introUIGobj.SetActive(visibility);
    }

    private void UpdateKesoAmount(int amount) {
        kesoAmount.text = amount.ToString();
    }
    
    public void OnClickBackButton() {
        characterSelectUI.gameObject.SetActive(false);
        introUIGobj.SetActive(true);
    }
}
