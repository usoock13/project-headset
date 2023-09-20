using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {
    [SerializeField] private GameObject characterSelectUI;
    [SerializeField] private GameObject introUI;
    
    public void OnClickIntroUI() {

    }
    private void ShowCharacterSelectUI(bool visibility) {
        characterSelectUI.SetActive(visibility);
    }
    private void ShowIntroUI(bool visibility) {
        introUI.SetActive(visibility);
    }
}
