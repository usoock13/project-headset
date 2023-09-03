using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterSpawner : MonoBehaviour {
    private float spawnDistance = 12f;
    
    protected void SpawnMonster(Monster monster, int amount) {
        Vector2 spawnPoint = new Vector2(spawnDistance, 0);
    }
    private void Update() {
        // Time.time
    }

    [Serializable]
    struct Scenario : IComparable<Scenario> {
        [SerializeField] float time;
        [SerializeField] UnityEvent action;

        public int CompareTo(Scenario other)
        {
            throw new NotImplementedException();
        }
    }
}