using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : MonoBehaviour {
    internal bool isGameOver = false;
    [SerializeField] private Transform characterSpawnPoint;
    [SerializeField] private Character __testCharacter;
    private Character character;
    public Character Character {
        get { return character; }
        set { character ??= value; }
    }
    [SerializeField] private ScenarioDirector scenarioDirector;
    public ScenarioDirector ScenarioDirector {
        get { return scenarioDirector; }
        set { scenarioDirector ??= value; }
    }
    [SerializeField] private EquipmentsManager equipmentsManager;
    public EquipmentsManager EquipmentsManager {
        get { return equipmentsManager; }
    }
    [SerializeField] private StageUIManager stageUIManager;
    public StageUIManager StageUIManager {
        get { return stageUIManager; }
    }

    [SerializeField] private Camera mainCamera;

    [SerializeField] private ItemCollector itemCollector;
    
    [SerializeField] private Sprite[] expSprites = new Sprite[2];
    [SerializeField] private ExpJewel expJewel;
    private ObjectPooler expPooler;

    private void Awake() {
        if(GameManager.instance.StageManager == null)
            GameManager.instance.StageManager = this;
        else
            Destroy(gameObject);
    }
    private void Start() {
        SpawnCharacter();
        
        mainCamera.transform.SetParent(character.transform);
        mainCamera.transform.localPosition = new Vector3(0, 0, mainCamera.transform.position.z);
        // Set Camera
        
        character.ItemCollector = itemCollector;
        // Set the object that get item to chase a character.

        scenarioDirector ??= FindObjectOfType<ScenarioDirector>();
        expPooler = new ObjectPooler(expJewel.gameObject, null, null, this.transform, 100, 50);
    }
    private void SpawnCharacter() {
        List<Character> selectedCharacter = GameManager.instance.selectedCharacters ?? new List<Character>() { __testCharacter };
        character = Instantiate(selectedCharacter[0].gameObject, characterSpawnPoint.position, characterSpawnPoint.rotation).GetComponent<Character>();

        Transform headmountPoint = character.headmountPoint;
        for(int i=1; i<selectedCharacter.Count; i++) {
            HeadmountCharacter hmc = Instantiate<HeadmountCharacter>(selectedCharacter[i].HeadmountCharacter, headmountPoint);
            headmountPoint = hmc.headmountPoint;
            hmc.gameObject.SetActive(true);
        }
    }
    public void OnCharacterLevelUp() {
        LevelUpUI levelUpUI = StageUIManager.LevelUpUI;
        IPlayerGettable[] choises = EquipmentsManager.RandomChoises(4);
        levelUpUI.SetChoise(0, choises[0]);
        levelUpUI.SetChoise(1, choises[1]);
        levelUpUI.SetChoise(2, choises[2]);
        levelUpUI.SetChoise(3, choises[3]);
        levelUpUI.ActiveUI();
    }
    public void GameOver() {
        isGameOver = true;
        scenarioDirector.OnEndsScenario();
    }
    public void CreateExp(Vector2 point, int expAmount) {
        var exp = expPooler.OutPool(point, Quaternion.identity).GetComponent<ExpJewel>();
        if(exp != null) {
            exp.givingExp = expAmount;
            exp.Drop();
        }
    }
}