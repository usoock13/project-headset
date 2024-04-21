using UnityEngine;

public class EscapeMenuUI : MonoBehaviour {
    [SerializeField] private SettingMenuUI settingMenu;

    public void Open() {
        this.gameObject.SetActive(true);
    }
    public void Close() {
        this.gameObject.SetActive(false);
    }

    #region Functions for events
    public void OnClickSettingButton() {
        settingMenu.Open();
    }
    public void OnClickCloseButton() {
        Close();
    }
    #endregion Functions for events
}