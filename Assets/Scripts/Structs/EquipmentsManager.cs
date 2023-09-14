using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentsManager : MonoBehaviour {
    Character Character {
        get { return GameManager.instance.StageManager.Character; }
    }

    [SerializeField] private List<Weapon> remainingWeapon;
    [SerializeField] private List<Weapon> havingWeapon;
    protected const int MAX_WEAPONS_COUNT = 6;
    [SerializeField] private List<Artifact> remainingArtifact;
    [SerializeField] private List<Artifact> havingArtifact;
    protected const int MAX_ACCESSORIES_COUNT = 6;

    private void Awake() {
        remainingWeapon = transform.GetComponentsInChildren<Weapon>(true).ToList();
        remainingArtifact = transform.GetComponentsInChildren<Artifact>(true).ToList();
        havingWeapon = new List<Weapon>();
        havingArtifact = new List<Artifact>();
    }
    public T[] RandomChoises<T>(int number) where T : Equipment {
        List<T> targetList;

        if(typeof(T) == typeof(Weapon))
            targetList = remainingWeapon as List<T>;
        else if(typeof(T) == typeof(Artifact))
            targetList = remainingArtifact as List<T>;
        else return null;
        int n = Mathf.Min(number, targetList.Count);
            
        targetList.Sort((a, b) => {
            return UnityEngine.Random.Range(-1, 1)<0 ? -1 : 1;
        });
        T[] result = new T[number];
        for(int i=0; i<n; i++) {
            result[i] = targetList[i];
        }
        return result;
    }
    
    public void GivePlayerEquipment(Equipment equipment) {
        if(equipment is Weapon) {
            int targetIndex = remainingWeapon.IndexOf(equipment as Weapon);
            if(targetIndex >= 0) {
                Weapon target = remainingWeapon[targetIndex];
                havingWeapon.Add(target);
                remainingWeapon.Remove(target);
                Character.AddWeapon(target);
            }
        } else if(equipment is Artifact) {
            int targetIndex = remainingArtifact.IndexOf(equipment as Artifact);
            if(targetIndex >= 0) {
                Artifact target = remainingArtifact[targetIndex];
                havingArtifact.Add(target);
                remainingArtifact.Remove(target);
                Character.AddArtifact(target);
            }
        }
    }
}