using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioGrassDirector : ScenarioDirector {
    [SerializeField] private Monster defaultMonster;
    [SerializeField] private int defaultSpawncount = 1;
    [SerializeField] private float defaultSpawnInterval = 1f;
    private int defaultMonsterDefeatCount = 0;

    private Coroutine spawnDefaultMonsterCoroutine;

    [SerializeField] private MonsterWolf monsterWolf;
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
                    count: 10,
                    restoreCount: 20
                )
            },
        };
        // poolerToad = new Dictionary<string, ObjectPooler>();
        // poolerUndead = new Dictionary<string, ObjectPooler>();
        // poolerWitch = new Dictionary<string, ObjectPooler>();

        scenarios.Add(new Scenario(3, () => {
            defaultMonster = monsterWolf;
            defaultSpawnInterval = 1f;
            defaultSpawncount = 3;
            spawnDefaultMonsterCoroutine = StartCoroutine(SpawnDefaultMonster());
        }));
    }

    protected IEnumerator SpawnDefaultMonster() {
        while(!ScenarioIsEnd) {
            SpawnMonster(defaultMonster, defaultSpawncount);
            yield return new WaitForSeconds(defaultSpawnInterval);
        }
    }

    public override void MonsterDefeatEvent(Monster monster) {
        if(++defaultMonsterDefeatCount%4 == 0) {
            SpawnMonster(defaultMonster, 1);
        }
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