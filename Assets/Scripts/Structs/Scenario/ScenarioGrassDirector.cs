using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    Dictionary<string, ObjectPooler> monsterPoolerMap;
    
    protected override void InitializeScenario() {
        monsterPoolerMap = new Dictionary<string, ObjectPooler> {
            {
                monsterWolf.MonsterType,
                new ObjectPooler(
                    monsterWolf.gameObject,
                    (monster) => { monster.GetComponent<Monster>()?.InitializeMonster(); },
                    null,
                    this.transform,
                    200, 10
                )
            },
        };
        // poolerToad = new Dictionary<string, ObjectPooler>();
        // poolerUndead = new Dictionary<string, ObjectPooler>();
        // poolerWitch = new Dictionary<string, ObjectPooler>();

        scenarios.Add(new Scenario(0, () => {
            defaultMonster = monsterWolf;
            defaultSpawncount = 10;
            defaultSpawnInterval = 1f;
            spawnDefaultMonsterCoroutine = StartCoroutine(SpawnDefaultMonster());
        }));
    }
    int count = 0;

    protected IEnumerator SpawnDefaultMonster() {
        while(!ScenarioIsEnd) {
            yield return new WaitForSeconds(defaultSpawnInterval);
            SpawnMonster(defaultMonster, defaultSpawncount);
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

    protected override void SpawnMonster(Monster monster, int amount) {
        ObjectPooler pooler;
        if(monsterPoolerMap.TryGetValue(monster.MonsterType, out pooler)) {
            for(int i=0; i<amount; i++) {
                Vector2 spawnPoint = RandomDirection * spawnDistance + (Vector2)Character.transform.position;
                GameObject m = pooler.OutPool();
                m.transform.position = spawnPoint;
            }
        }
        count += 10;
        print(count);
    }
}