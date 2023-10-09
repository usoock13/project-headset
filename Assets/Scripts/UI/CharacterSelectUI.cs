using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class CharacterSelectUI : MonoBehaviour {
    [SerializeField] private Character[] characterList;
    [SerializeField] private List<Character> selectedCharacters;
    const int CHARACTER_SELECT_LIMIT = 3;

    [SerializeField] private Button[] choiseItemButtons;
    [SerializeField] private ChoiseItem[] choiseItems;

    [SerializeField] private Transform selectedCharacterShowPoint;

    [SerializeField] private SpriteRenderer bottomCharacterSprite;
    [SerializeField] private HeadmountCharacterShower[] headmountCharacters;
    [SerializeField] private TMP_Text selectedCountText;

    #region Unity Events
    private void Start() {
        InitializeChoise();
        for(int i=0; i<choiseItems.Length && i<characterList.Length; i++) {
            SetChoise(i, characterList[i]);
        }
        UpdateSelectedCharacterViewer();
    }
    #endregion Unity Events
    private void InitializeChoise() {
        choiseItems = new ChoiseItem[choiseItemButtons.Length];
        for(int i=0; i<choiseItemButtons.Length; i++) {
            choiseItems[i] = new ChoiseItem() {
                targetButton = choiseItemButtons[i],
                characterImage = choiseItemButtons[i].transform.GetComponentsInChildren<Image>()[1],
                characterName = choiseItemButtons[i].transform.GetComponentsInChildren<TMP_Text>()[0],
            };
            int index = i;
            choiseItems[i].targetButton.onClick.AddListener(() => { SelectCharacter(index); });
        }
    }

    public void SelectCharacter(int index) {
        Character target = characterList[index];
        if(!selectedCharacters.Contains(target)) {
            if(selectedCharacters.Count < CHARACTER_SELECT_LIMIT) {
                selectedCharacters.Add(target);
            } else {
                throw new NotImplementedException("캐릭터 선택 카운트 한도 초과");
            }
        } else {
            selectedCharacters.Remove(target);
        }
        GameManager.instance.selectedCharacters = selectedCharacters;
        UpdateSelectedCharacterViewer();
    }

    private void UpdateSelectedCharacterViewer() {
        bottomCharacterSprite.sprite = null;
        for(int i=0; i<headmountCharacters.Length; i++) {
            headmountCharacters[i].hands.sprite = null;
            headmountCharacters[i].front.sprite = null;
            headmountCharacters[i].back.sprite = null;
        }
        for(int i=0; i<selectedCharacters.Count; i++) {
            if(i == 0) {
                bottomCharacterSprite.sprite = selectedCharacters[i].DefaultSprite;
            } else {
                HeadmountCharacter hmc = selectedCharacters[i].HeadmountCharacter;
                headmountCharacters[i-1].hands.sprite = hmc.HandsSprite.sprite;
                headmountCharacters[i-1].front.sprite = hmc.FrontSprite.sprite;
                headmountCharacters[i-1].back.sprite = hmc.BackSprite.sprite;
            }
        }
        selectedCountText.text = $"{selectedCharacters.Count} / {CHARACTER_SELECT_LIMIT}";
    }

    private void SetChoise(int index, Character character) {
        choiseItems[index].characterImage.sprite = character.DefaultSprite;
        choiseItems[index].characterName.text = character.CharacterName;
    }

    [System.Serializable]
    public class ChoiseItem {
        public Button targetButton;
        public Image characterImage;
        public TMP_Text characterName;
    }
    [System.Serializable]
    public class HeadmountCharacterShower {
        public SpriteRenderer hands;
        public SpriteRenderer front;
        public SpriteRenderer back;
    }
}