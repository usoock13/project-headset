using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

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
    public List<Character> Party { get; private set; }

    private PlayerInput PlayerInput => character.GetComponent<PlayerInput>();
    [SerializeField] private ScenarioDirector scenarioDirector;
    public ScenarioDirector ScenarioDirector => scenarioDirector;

    [SerializeField] private EquipmentManager equipmentsManager;
    public EquipmentManager EquipmentsManager => equipmentsManager;

    [SerializeField] private StageUIManager stageUIManager;
    public StageUIManager StageUIManager => stageUIManager;

    [SerializeField] private GameObject characterCamera;
    [SerializeField] private CameraDirector cameraDirector;
    public CameraDirector CameraDirector => cameraDirector;

    #region Item
    [SerializeField] private ItemCollector itemCollector;
    private const int MAX_KESO_FALLEN_COUNT = 200;
    private const int MAX_EXP_FALLEN_COUNT = 200;
    
    [SerializeField] private ExpJewel expJewel;
    private ObjectPooler expPooler;

    [SerializeField] private Meat meatOrigin;
    private ObjectPooler MeatPooler;

    [SerializeField] private Keso kesoOrigin;
    private ObjectPooler kesoPooler;

    [SerializeField] private Salad saladOrigin;
    private ObjectPooler saladPooler;
    #endregion Item

    [SerializeField] private DamagePrinter damagePrinter;

    public int KillScore { get; private set; } = 0;
    public int KesoEarned { get; private set; } = 0;

    public float StageLevel { get; private set; } = 1f;
    /// <summary>
    /// float : Stage level value before stage level up.
    /// float : Stage level value after atage level up.
    /// </summary>
    public Action<float, float> onChageStageLevel;

    [SerializeField] private Light2D globalLight;
    [SerializeField] private Color sunLightColor = Color.white;
    [SerializeField] private Color moonLightColor = Color.gray;

    [SerializeField] private AudioClip stageBgmClip;
    
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
        
        characterCamera.transform.SetParent(character.transform);
        characterCamera.transform.localPosition = new Vector3(0, 0, characterCamera.transform.position.z);
        // Set Camera
        
        character.ItemCollector = itemCollector;
        // Set the object that get item to chase a character.

        scenarioDirector ??= FindObjectOfType<ScenarioDirector>();
        expPooler = new ObjectPooler(expJewel.gameObject, null, null, parent: this.transform);
        MeatPooler = new ObjectPooler(meatOrigin.gameObject, null, null, parent: this.transform);
        kesoPooler = new ObjectPooler(kesoOrigin.gameObject, null, null, null, parent: this.transform);
        saladPooler = new ObjectPooler(saladOrigin.gameObject, null, null, parent: this.transform);

        GameManager.instance.SoundManager.PlayBGM(stageBgmClip);
    }

    public void IncreaseStageLevel(float amount) {
        float prev = StageLevel;
        StageLevel += amount;
        OnIncreaseStageLevel(prev, StageLevel);
    }
    public void ChangeDayNight(bool isDay) {
        character.TurnOnLamp(!isDay);
        StartCoroutine(ChangeDayNightCoroutine(isDay));
    }
    public IEnumerator ChangeDayNightCoroutine(bool isDay) {
        if(globalLight != null) {
            float offset = 0;
            Color start = globalLight.color;
            Color end = isDay ? sunLightColor : moonLightColor;
            while(offset < 1) {
                globalLight.color = Color.Lerp(start, end, offset);
                offset += Time.deltaTime * 0.5f;
                yield return null;
            }
            globalLight.color = end;
        }
    }

    private void OnIncreaseStageLevel(float prev, float next) {
        onChageStageLevel?.Invoke(prev, next);
    }

    private void SpawnCharacter() {
        List<Character> selectedCharacter = GameManager.instance.SelectedCharacters ?? __testCharacters;
        Party = selectedCharacter;

        character = Instantiate(selectedCharacter[0].gameObject, characterSpawnPoint.position, characterSpawnPoint.rotation).GetComponent<Character>();
        
        character.HeadmountCharacter.HeadAbility?.transform.SetParent(character.transform);
        character.HeadmountCharacter.HeadAbility?.OnTaken(character);

        for(int i=1; i<selectedCharacter.Count; i++) {
            character.MountCharacter(selectedCharacter[i].HeadmountCharacter);
        }
    }

    private void InitializeUI() {
        stageUIManager.InitializeStatusUI();
    }

    public void OnCharacterLevelUp() {
        if(character.CurrentState.Compare(character.dieState))
            return;
            
        StageUIManager.LevelUpUI.ShowChoise();
    }
    public void GameOver() {
        isGameOver = true;
        scenarioDirector.OnEndsScenario();
        GameManager.instance.ProfileManager.IncreaseKeso(KesoEarned);
    }

    #region Create Item
    public void CreateExp(Vector2 point, int expAmount) {
        var exp = expPooler.OutPool(point, Quaternion.identity).GetComponent<ExpJewel>();
        if(exp != null) {
            exp.GivingExp = expAmount;
            exp.Drop();
        }
    }
    public void CreateKeso(Vector2 point, int kesoAmount) {
        GameObject instan = kesoPooler.OutPool(point, Quaternion.identity);
        if(instan.TryGetComponent(out Keso keso)) {
            keso.Amount = kesoAmount;
            keso.Drop();
        }
    }
    public void CreateMeat(Vector2 point) {
        GameObject instance = MeatPooler.OutPool(point, Quaternion.identity);
        if(instance.TryGetComponent(out Meat meat)) {
            meat.Drop();
        }
    }
    public void CreateSalad(Vector2 point) {
        GameObject instance = saladPooler.OutPool(point, quaternion.identity);
        if(instance.TryGetComponent(out Salad salad)) {
            salad.Drop();
        }
    }
    #endregion Create Item

    public void OnGetItem(Item item) {
        Character.onGetItem?.Invoke(item);
    }
    
    public void OnMonsterDie(Monster monster) {
        character.OnKillMonster(monster);
        scenarioDirector.MonsterDefeatHandler(monster);
        IncreaseKillScore();
    }
    public void PauseGame(bool pause) {
        isGamePause = pause;
        Time.timeScale = pause ? 0 : 1;
        // if(pause)
        //     PlayerInput.actions.actionMaps[0].Disable();
        // else
        //     PlayerInput.actions.actionMaps[0].Enable();
    }
    static public void SetTimeScale(float scale) {
        if(!isGamePause)
            Time.timeScale = scale;
    }
    public void PrintDamageNumber(Vector2 point, string number) {
        this.PrintDamageNumber(point, number, Color.white);
    }
    public void PrintDamageNumber(Vector2 point, string number, Color color) {
        Vector2 pos = point + new Vector2(UnityEngine.Random.Range(-.3f, .3f), 0);
        damagePrinter.PrintDamage(pos, number, color);
    }
    private void IncreaseKillScore() {
        KillScore ++;
        stageUIManager.UpdateProgressingBoard();
    }
    public void IncreaseKesoEarned(int amount) {
        KesoEarned += amount;
        stageUIManager.UpdateProgressingBoard();
    }
}