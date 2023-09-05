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
    private ScenarioDirector scenarioDirector;
    public ScenarioDirector ScenarioDirector {
        get { return scenarioDirector; }
        set { scenarioDirector ??= value; }
    }

    private void Awake() {
        if(GameManager.instance.StageManager == null)
            GameManager.instance.StageManager = this;
        else
            Destroy(gameObject);
    }
    private void Start() {
        character = Instantiate(__testCharacter.gameObject, characterSpawnPoint.position, characterSpawnPoint.rotation).GetComponent<Character>();
        scenarioDirector ??= FindObjectOfType<ScenarioDirector>();
    }
    public void GameOver() {
        isGameOver = true;
        scenarioDirector.OnEndsScenario();
    }
}