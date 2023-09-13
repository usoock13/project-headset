using UnityEngine;

public class UIManager : MonoBehaviour {
    [SerializeField] private LevelUpUI levelUpUI;
    public LevelUpUI LevelUpUI { get; private set; }

    private void Start() {
        GameManager.instance.onLevelUp += () => {
            
        };
    }
}