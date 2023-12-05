using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LevelUpUI : MonoBehaviour {
    [SerializeField] private ChoiseItem[] choiseItemUIs = new ChoiseItem[4];
    [SerializeField] private IPlayerGettable[] choises = new IPlayerGettable[4];
    [SerializeField] private Animator animator;
    public UnityEvent onActive;

    public void ActiveUI() {
        this.gameObject.SetActive(true);
        GameManager.instance.StageManager.PauseGame(true);
        onActive?.Invoke();
    }
    public void InactiveUI() {
        this.gameObject.SetActive(false);
        GameManager.instance.StageManager.PauseGame(false);
    }
    public void SetChoise(int index, IPlayerGettable choise) {
        if(index > choiseItemUIs.Length)
            throw new Exception($"Index is over the max choises count.\n(max count {index} / received {index})");

        choiseItemUIs[index].SetItem(choise);
        choises[index] = choise;
    }
    public void SelectChoise(int index) {
        var character = GameManager.instance.Character;
        var stageManager = GameManager.instance.StageManager;
        stageManager.EquipmentsManager.GivePlayerItem(choises[index]);
        this.InactiveUI();
        if(++character.levelRewardCount < character.level-1)
            stageManager.OnCharacterLevelUp();
            
    }
    [System.Serializable]
    private struct ChoiseItem {
        public Image icon;
        public TMP_Text name;
        public TMP_Text description;
        public void SetItem(IPlayerGettable info) {
            this.icon.sprite = info.Icon;
            this.name.text = info.Name;
            this.description.text = info.Description;
        }
    }
}