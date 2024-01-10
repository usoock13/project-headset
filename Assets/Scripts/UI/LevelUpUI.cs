using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class LevelUpUI : MonoBehaviour {
    [SerializeField] private ChoiseItem[] choiseItemUIs = new ChoiseItem[4];
    [SerializeField] private IPlayerGettable[] choises = new IPlayerGettable[4];
    [SerializeField] private Animator animator;
    public UnityEvent onActive;

    private const int defaultRerollPrice = 1000;
    private int rerollPrice = defaultRerollPrice;
    [SerializeField] private TMP_Text rerollPriceText;
    private Color rerollPriceColorOrigin;
    private Coroutine deficientAnimationCoroutine;

    private void Start() {
        rerollPriceColorOrigin = rerollPriceText.color;
    }

    public void ActiveUI() {
        this.gameObject.SetActive(true);
        GameManager.instance.StageManager.PauseGame(true);
        GameManager.instance.InputSystem.ChangeToUIMode();
        onActive?.Invoke();
    }
    public void InactiveUI() {
        this.gameObject.SetActive(false);
        GameManager.instance.StageManager.PauseGame(false);
        GameManager.instance.InputSystem.ChangeToControlMode();
    }
    public void ShowChoise() {
        var list = GameManager.instance.StageManager.EquipmentsManager.RandomChoises(4);
        SetChoise(0, list[0]);
        SetChoise(1, list[1]);
        SetChoise(2, list[2]);
        SetChoise(3, list[3]);
        rerollPriceText.text = $"{rerollPrice:0,0}";
        ActiveUI();
    }

    private IEnumerator DeficientAnimation() {
        Color start = new Color(1f, 0.2f, 0.2f);
        Color end = rerollPriceColorOrigin;
        float offset = 0;
        while(offset < 1) {
            offset += Time.unscaledDeltaTime * 5f;
            rerollPriceText.color = Color.Lerp(start, end, offset);
            yield return null;
        }
        rerollPriceText.color = end;
    }

    public void Reroll() {
        if(GameManager.instance.StageManager.KesoEarned >= rerollPrice) {
            GameManager.instance.StageManager.IncreaseKesoEarned(-rerollPrice);
            rerollPrice *= 2;
            ShowChoise();
        } else {
            if(deficientAnimationCoroutine != null)
                StopCoroutine(deficientAnimationCoroutine);
            deficientAnimationCoroutine = StartCoroutine(DeficientAnimation());
        }
    }

    private void SetChoise(int index, IPlayerGettable choise) {
        if(index > choiseItemUIs.Length)
            throw new Exception($"Index is over the max choises count.\n(max count {index} / received {index})");

        choiseItemUIs[index].SetItem(choise);
        choises[index] = choise;
    }

    public void SelectChoise(int index) {
        var character = GameManager.instance.Character;
        var stageManager = GameManager.instance.StageManager;
        stageManager.EquipmentsManager.OnPlayerSelectItem(choises[index]);
        this.InactiveUI();
        if(++character.levelRewardCount < character.level-1)
            stageManager.OnCharacterLevelUp();
        rerollPrice = defaultRerollPrice;
    }

    [System.Serializable]
    private struct ChoiseItem {
        public Image icon;
        public TMP_Text name;
        public TMP_Text description;
        public TMP_Text level;
        public void SetItem(IPlayerGettable info) {
            this.icon.sprite = info.Icon;
            this.name.text = info.Name;
            this.description.text = info.Description;

            var equipment = info as Equipment;
            if(equipment is not null)
                level.text = equipment.CurrentLevel + 1 + "Lv";
            else
                level.text = "";
        }
    }
}