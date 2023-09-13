using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class LevelUpUI : MonoBehaviour {
    ChoiseItem[] choiseItems = new ChoiseItem[4];

    public void ActiveUI() {
        this.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void InactiveUI() {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void SetChoise(int index, Equipment equipment) {
        if(index > choiseItems.Length) {
            throw new Exception($"Index is over the max choises count.\n(max count {index} / received {index})");
        }

        ChoiseInformation info = new ChoiseInformation(
                                    equipment.Icon,
                                    equipment.Name,
                                    equipment.Description);
        choiseItems[index].button.onClick.AddListener(() => {
            equipment.OnEquipped();
            choiseItems[index].button.onClick.RemoveAllListeners();
        });
        
    }

    public struct ChoiseInformation {
        private Sprite _itemIcon;
        private string _itemName;
        private string _itemDescription;
        
        public ChoiseInformation(Sprite icon, string name, string description) {
            this._itemIcon = icon;
            this._itemName = name;
            this._itemDescription = description;
        }
    }
    [System.Serializable]
    public struct ChoiseItem {
        public Button button;
        public Image icon;
        public TextMeshPro name;
        public TextMeshPro description;
    }
}