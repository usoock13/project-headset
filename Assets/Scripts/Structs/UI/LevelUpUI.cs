using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour {
    [SerializeField] private ChoiseItem[] choiseItems = new ChoiseItem[4];
    [SerializeField] private Equipment[] choiseEquipments = new Equipment[4];

    public void ActiveUI() {
        this.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void InactiveUI() {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void SetChoise(int index, Equipment equipment) {
        if(index > choiseItems.Length)
            throw new Exception($"Index is over the max choises count.\n(max count {index} / received {index})");

        ChoiseInformation info = new ChoiseInformation(
                                    equipment.Icon,
                                    equipment.Name,
                                    equipment.Description);
        choiseItems[index].SetItem(info);
        choiseEquipments[index] = equipment;
    }
    public void SelectChoise(int index) {
        GameManager.instance.StageManager.EquipmentsManager.GivePlayerEquipment(choiseEquipments[index]);
        this.InactiveUI();
    }

    public struct ChoiseInformation {
        public Sprite itemIcon;
        public string itemName;
        public string itemDescription;

        public ChoiseInformation(Sprite icon, string name, string description) {
            this.itemIcon = icon;
            this.itemName = name;
            this.itemDescription = description;
        }
    }
    [System.Serializable]
    private struct ChoiseItem {
        public Image icon;
        public TMP_Text name;
        public TMP_Text description;
        public void SetItem(ChoiseInformation info) {
            this.icon.sprite = info.itemIcon;
            this.name.text = info.itemName;
            this.description.text = info.itemDescription;
        }
    }
}