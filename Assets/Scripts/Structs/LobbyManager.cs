using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    [SerializeField] private IntroUI introUI;
    [SerializeField] private CharacterSelectUI characterSelectUI;
    [SerializeField] private GameObject introUIGobj;

    [SerializeField] private TMP_Text kesoAmount;

    private void Start() {
        InitializeUIs();
    }

    private void InitializeUIs() {
        UpdateKesoAmount(GameManager.instance.ProfileManager.HavingKeso);
    }

    public void OnClickIntroUI() {
        ShowIntroUI(false);
        ShowCharacterSelectUI(true);
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

    private void OnClickBackButton() {
        characterSelectUI.gameObject.SetActive(false);
        introUI.gameObject.SetActive(true);
    }
}
