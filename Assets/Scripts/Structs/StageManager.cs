using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : MonoBehaviour {
    internal bool isGameOver = false;
    static public bool isGamePause = false;
    [SerializeField] private Transform characterSpawnPoint;
    [SerializeField] private List<Character> __testCharacters;
    private Character character;
    public Character Character {
        get { return character; }
        set { character ??= value; }
    }
    private PlayerInput PlayerInput => character.GetComponent<PlayerInput>();
    [SerializeField] private ScenarioDirector scenarioDirector;
    public ScenarioDirector ScenarioDirector {
        get { return scenarioDirector; }
        set { scenarioDirector ??= value; }
    }
    [SerializeField] private EquipmentManager equipmentsManager;
    public EquipmentManager EquipmentsManager {
        get { return equipmentsManager; }
    }
    [SerializeField] private StageUIManager stageUIManager;
    public StageUIManager _StageUIManager {
        get { return stageUIManager; }
    }

    [SerializeField] private Camera mainCamera;

    [SerializeField] private ItemCollector itemCollector;
    
    [SerializeField] private Sprite[] expSprites = new Sprite[2];
    [SerializeField] private ExpJewel expJewel;
    private ObjectPooler expPooler;

    [SerializeField] private Potion potionOrigin;
    private ObjectPooler potionPooler;

    [SerializeField] private DamagePrinter damagePrinter;

    private void Awake() {
        if(GameManager.instance.StageManager == null)
            GameManager.instance.StageManager = this;
        else
            Destroy(gameObject);
    }
    private void Start() {
        SpawnCharacter();
        character.InitializeCharacter();
        InitializeUI();
        
        mainCamera.transform.SetParent(character.transform);
        mainCamera.transform.localPosition = new Vector3(0, 0, mainCamera.transform.position.z);
        // Set Camera
        
        character.ItemCollector = itemCollector;
        // Set the object that get item to chase a character.

        scenarioDirector ??= FindObjectOfType<ScenarioDirector>();
        expPooler = new ObjectPooler(expJewel.gameObject, null, null, this.transform, 100, 50);
        potionPooler = new ObjectPooler(potionOrigin.gameObject, null, null, this.transform, 50, 20);
    }
    private void SpawnCharacter() {
        List<Character> selectedCharacter = GameManager.instance.SelectedCharacters ?? __testCharacters;
        character = Instantiate(selectedCharacter[0].gameObject, characterSpawnPoint.position, characterSpawnPoint.rotation).GetComponent<Character>();
        character.HeadmountCharacter.HeadAbility?.OnTaken(character);

        for(int i=1; i<selectedCharacter.Count; i++) {
            character.MountCharacter(selectedCharacter[i].HeadmountCharacter);
        }
    }
    private void InitializeUI() {
        stageUIManager.InitializeStatusUI();
    }
    public void OnCharacterLevelUp() {
        LevelUpUI levelUpUI = _StageUIManager.LevelUpUI;
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
    public void CreatePotion(Vector2 point) {
        GameObject instance = potionPooler.OutPool(point, Quaternion.identity);
        if(instance.TryGetComponent(out Potion potion)) {
            potion.Drop();
        }
    }
    public void OnMonsterDie(Monster monster) {
        character.OnKillMonster(monster);
    }
    public void PauseGame(bool pause) {
        Time.timeScale = pause ? 0 : 1;
        // if(pause)
        //     PlayerInput.actions.actionMaps[0].Disable();
        // else
        //     PlayerInput.actions.actionMaps[0].Enable();
        isGamePause = pause;
    }
    public void PrintDamageNumber(Vector2 point, string number) {
        this.PrintDamageNumber(point, number, Color.white);
    }
    public void PrintDamageNumber(Vector2 point, string number, Color color) {
        Vector2 pos = point + new Vector2(UnityEngine.Random.Range(-.3f, .3f), 0);
        damagePrinter.PrintDamage(pos, number, color);
    }
}