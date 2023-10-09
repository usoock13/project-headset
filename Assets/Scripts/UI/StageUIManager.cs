using UnityEngine;

public class StageUIManager : MonoBehaviour {
    [SerializeField] private LevelUpUI levelUpUI;
    public LevelUpUI LevelUpUI => levelUpUI;
    
    [SerializeField] private CharacterStatusUI characterUI;
    public CharacterStatusUI CharacterStatusUI => characterUI;
}