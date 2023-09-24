using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {
    [SerializeField] private CharacterSelectUI characterSelectUI;
    [SerializeField] private GameObject introUIGobj;
    
    public void OnClickIntroUI() {
        ShowIntroUI(false);
        ShowCharacterSelectUI(true);
    }
    private void ShowCharacterSelectUI(bool visibility) {
        characterSelectUI.gameObject.SetActive(visibility);
    }
    private void ShowIntroUI(bool visibility) {
        introUIGobj.SetActive(visibility);
    }
}
