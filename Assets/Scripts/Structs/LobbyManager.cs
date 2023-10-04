using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    [SerializeField] private CharacterSelectUI characterSelectUI;
    [SerializeField] private GameObject introUIGobj;
    
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
}
