using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioGrassDirector : ScenarioDirector {
    [SerializeField] private const int MAX_MONSTER_COUNT = 200;
    [SerializeField] private Monster defaultMonster;
    [SerializeField] private int defaultSpawncount = 1;
    [SerializeField] private float defaultSpawnInterval = 1f;
    private int monsterSpawnedCount = 0;

    private Coroutine spawnDefaultMonsterCoroutine;

    [SerializeField] private MonsterBasic monsterWolf;
    [SerializeField] private MonsterBasic monsterBear;
    [SerializeField] private MonsterToad monsterToad;
    [SerializeField] private MonsterUndead monsterUndead;
    [SerializeField] private MonsterWitch monsterWitch;
    
    protected override void InitializeScenario() {
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
                monsterBear.MonsterType,
                new ObjectPooler(
                    monsterBear.gameObject,
                    parent: this.transform,
                    count: 10,
                    restoreCount: 2
                )
            },
        };
        // poolerToad = new Dictionary<string, ObjectPooler>();
        // poolerUndead = new Dictionary<string, ObjectPooler>();
        // poolerWitch = new Dictionary<string, ObjectPooler>();

        scenarios.Add(new Scenario(3, () => {
            defaultMonster = monsterWolf;
            defaultSpawnInterval = 1.5f;
            defaultSpawncount = 5;
            spawnDefaultMonsterCoroutine = StartCoroutine(SpawnDefaultMonster());
        }));
        scenarios.Add(new Scenario(5, () => {
            SpawnMonster(monsterBear, 1);
        }));
    }

    protected IEnumerator SpawnDefaultMonster() {
        while(!ScenarioIsEnd) {
            if(monsterSpawnedCount < MAX_MONSTER_COUNT) {
                SpawnMonster(defaultMonster, defaultSpawncount);
                monsterSpawnedCount += defaultSpawncount;
            }
            yield return new WaitForSeconds(defaultSpawnInterval);
        }
    }

    public override void MonsterDefeatHandler(Monster monster) {
        monsterSpawnedCount--;
    }

    public override void OnEndsScenario() {
        StopCoroutine(spawnDefaultMonsterCoroutine);
    }

    protected override List<Monster> SpawnMonster(Monster monster, int amount) {
        ObjectPooler pooler;
        if(monsterPoolerMap.TryGetValue(monster.MonsterType, out pooler)) {
            List<Monster> result = new List<Monster>();
            for(int i=0; i<amount; i++) {
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