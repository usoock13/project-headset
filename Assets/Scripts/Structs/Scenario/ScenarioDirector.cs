using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ScenarioDirector : MonoBehaviour {
    protected Character character;
    protected Character Character {
        get {
            character ??= GameManager.instance.Character;
            return character;
        }
    }
    protected float spawnDistance = 15f;
    protected List<Scenario> scenarios = new List<Scenario>();
    protected int order = 0;
    protected Scenario next;
    protected bool ScenarioIsEnd {
        get { return GameManager.instance.GameIsOver; }
    }
    protected Vector2 RandomDirection {
        get { return Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.forward) * Vector2.up; }
    }
    
    public Dictionary<string, ObjectPooler> monsterPoolerMap { get; protected set; }

    #region Unity Events
    protected virtual void Start() {
        character = GameManager.instance.Character;
        InitializeScenario();
        scenarios.Sort();
    }
    protected virtual void Update() {
        if(order < scenarios.Count
        && Time.time >= next.time) {
            next = scenarios[order++];
            next.action?.Invoke();
        }
    }
    #endregion Unity Events
    
    protected abstract void InitializeScenario();
    protected abstract List<Monster> SpawnMonster(Monster monster, int amount);

    [Serializable]
    public struct Scenario : IComparable<Scenario> {
        public float time;
        public UnityAction action;

        public Scenario(float time, UnityAction action) {
            this.time = time;
            this.action = action;
        }
        public int CompareTo(Scenario other) {
            if(this.time < other.time)
                return -1;
            else if(this.time > other.time)
                return 1;
            else
                return 0;
        }
    }
    public abstract void MonsterDefeatEvent(Monster monster);
    public abstract void OnEndsScenario();
}