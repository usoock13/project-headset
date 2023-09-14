using UnityEngine;

public class StageUIManager : MonoBehaviour {
    [SerializeField] private LevelUpUI levelUpUI;
    public LevelUpUI LevelUpUI {
        get { return levelUpUI; }
    }

    private void Start() {
        GameManager.instance.onLevelUp += () => {
            
        };
    }
}