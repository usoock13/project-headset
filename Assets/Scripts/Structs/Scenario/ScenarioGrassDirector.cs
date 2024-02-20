using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioGrassDirector : ScenarioDirector {
    [SerializeField] private const int MAX_MONSTER_COUNT = 200;
    private int monsterSpawnedCount = 0;

    [SerializeField] private MonsterBasic monsterWolf;
    [SerializeField] private MonsterBasic monsterBear;
    [SerializeField] private MonsterBasic monsterBat;
    [SerializeField] private MonsterBasic monsterGoblin;
    [SerializeField] private MonsterBasic monsterGoblinRider;

    [SerializeField] private Transform bossSummonPoint;
    [SerializeField] private MonsterWitch bossMonsterWitch;

    private StageManager _StageManager => GameManager.instance.StageManager;

    private bool bossIsSummoned = false;

    protected override void Update() {
        base.Update();
        
        if(Input.GetKeyDown(KeyCode.K))
            SummonBoss();
    }
    
    protected override void InitializeScenario() {
        // return;
        monsterPoolerMap = new Dictionary<string, ObjectPooler> {
            {
                monsterWolf.MonsterType,
                new ObjectPooler(
                    monsterWolf.gameObject,
                    parent: this.transform,
                    count: 100,
                    restoreCount: 20
                )
            },
            {
                monsterBat.MonsterType,
                new ObjectPooler(
                    monsterBat.gameObject,
                    parent: this.transform,
                    count: 100,
                    restoreCount: 20
                )
            },
            {
                monsterBear.MonsterType,
                new ObjectPooler(
                    monsterBear.gameObject,
                    parent: this.transform,
                    count: 10,
                    restoreCount: 2
                )
            },
            {
                monsterGoblin.MonsterType,
                new ObjectPooler(
                    monsterGoblin.gameObject,
                    parent: this.transform,
                    count: 100,
                    restoreCount: 20
                )
            },
            {
                monsterGoblinRider.MonsterType,
                new ObjectPooler(
                    monsterGoblinRider.gameObject,
                    parent: this.transform,
                    count: 50,
                    restoreCount: 10
                )
            },
        };
        // poolerToad = new Dictionary<string, ObjectPooler>();
        // poolerUndead = new Dictionary<string, ObjectPooler>();
        // poolerWitch = new Dictionary<string, ObjectPooler>();

        scenarios.Add(new Scenario(3, () => {
            IEnumerator AutoSpawn() {
                SpawnMonster(monsterBear, 10);
                while(!bossIsSummoned && Time.time < 30) {

                    SpawnDefaultMonster(monsterBat, 3);
                    SpawnDefaultMonster(monsterWolf, 1);
                    yield return new WaitForSeconds(3f);
                }
            }
            StartCoroutine(AutoSpawn());
        })); // # 01

        scenarios.Add(new Scenario(30, () => {
            IEnumerator AutoSpawn() {
                while(!bossIsSummoned && Time.time < 120) {
                    SpawnDefaultMonster(monsterGoblin, 4);
                    SpawnDefaultMonster(monsterGoblinRider, 1);
                    yield return new WaitForSeconds(3f);
                }
            }
            StartCoroutine(AutoSpawn());
        })); // # 02

        scenarios.Add(new Scenario(60, () => {
            SpawnMonster(monsterBear, 10);
            _StageManager.IncreaseStageLevel(0.5f);
            _StageManager.ChangeDayNight(false);
        })); // # 03

        scenarios.Add(new Scenario(120, () => {
            IEnumerator AutoSpawn() {
                while(!bossIsSummoned) {
                    SpawnDefaultMonster(monsterBear, 3);
                    yield return new WaitForSeconds(2f);
                }
            }
            StartCoroutine(AutoSpawn());
            _StageManager.IncreaseStageLevel(-0.5f);
            _StageManager.ChangeDayNight(true);
        })); // # 04 (Extra temporary spawn to test)
    }

    public void SummonBoss() {
        bossIsSummoned = true;
        /* 
            Actually summon boss.
        */
        MonsterWitch boss = Instantiate(bossMonsterWitch, bossSummonPoint.position, Quaternion.identity);
        boss.gameObject.SetActive(true);
    }

    protected void SpawnDefaultMonster(Monster monster, int number) {
        if(monsterSpawnedCount < MAX_MONSTER_COUNT) {
            SpawnMonster(monster, number);
        }
    }

    public override void MonsterDefeatHandler(Monster monster) {
        monsterSpawnedCount--;
    }

    public override void OnEndsScenario() {
        StopAllCoroutines();
    }

    protected override List<Monster> SpawnMonster(Monster monster, int number) {
        ObjectPooler pooler;
        monsterSpawnedCount += number;
        if(monsterPoolerMap.TryGetValue(monster.MonsterType, out pooler)) {
            List<Monster> result = new List<Monster>();
            for(int i=0; i<number; i++) {
                Vector2 spawnPoint = RandomDirection * spawnDistance + (Vector2)Character.transform.position;
                spawnPoint.x = Mathf.Clamp(spawnPoint.x, mapLeftBottom.x, mapRightTop.x);
                spawnPoint.y = Mathf.Clamp(spawnPoint.y, mapLeftBottom.y, mapRightTop.y);
                GameObject monsterInstance = pooler.OutPool(spawnPoint, Quaternion.identity);
                if(monsterInstance.TryGetComponent(out Monster m))
                    result.Add(m);
            }
            return result;
        }
        return null;
    }
}