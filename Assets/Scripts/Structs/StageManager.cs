using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
        character = Instantiate(__testCharacter.gameObject, characterSpawnPoint.position, characterSpawnPoint.rotation).GetComponent<Character>();
        scenarioDirector ??= FindObjectOfType<ScenarioDirector>();
        expPooler = new ObjectPooler(expJewel.gameObject, null, null, this.transform, 100, 50);
    }
    public void GameOver() {
        isGameOver = true;
        scenarioDirector.OnEndsScenario();
    }
    public void CreateExp(Vector2 point, float expAmount) {
        var exp = expPooler.OutPool(point, Quaternion.identity).GetComponent<ExpJewel>();
        if(exp != null) {
            exp.givingExp = expAmount;
            exp.Drop();
        }
    }
}