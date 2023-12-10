using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class EquipmentManager : MonoBehaviour {
    Character _Character => GameManager.instance.StageManager.Character;
    StageUIManager _UiManager => GameManager.instance.StageManager._StageUIManager;

    protected const int MAX_WEAPONS_COUNT = 6;
    protected const int MAX_ARTIFACTS_COUNT = 6;
    [SerializeField] private List<Weapon> remainingWeapons;
    [SerializeField] private List<Weapon> havingWeapons;
    public IEnumerable<Weapon> WeaponsEnumerator { get {
        for(int i=0; i<havingWeapons.Count; i++)
            yield return havingWeapons[i];
    }}

    [SerializeField] private List<Artifact> remainingArtifacts;
    [SerializeField] private List<Artifact> havingArtifacts;
    public IEnumerable<Artifact> ArtifactsEnumerator { get {
        for(int i=0; i<havingArtifacts.Count; i++)
            yield return havingArtifacts[i];
    }}

    [SerializeField] private Transform bonusChoisesParent;
    [SerializeField] private List<Item> bonusChoises;

    [SerializeField] private Weapon __testItem;

    private void Awake() {
        remainingWeapons = transform.GetComponentsInChildren<Weapon>(true).ToList();
        remainingArtifacts = transform.GetComponentsInChildren<Artifact>(true).ToList();
        havingWeapons = new List<Weapon>();
        havingArtifacts = new List<Artifact>();

        bonusChoises = bonusChoisesParent.GetComponentsInChildren<Item>(true).ToList();
    }
    #if UNITY_EDITOR
    
    #endif
    public IPlayerGettable[] RandomChoises(int number) {
        List<IPlayerGettable> candidates = new List<IPlayerGettable>();

        List<Weapon> weaponCandidates = new List<Weapon>();             // weapons character is having
        List<Artifact> artifactCandidates = new List<Artifact>();       // artifacts character is having
        List<Equipment> remainingEquipments = new List<Equipment>();    // all equipments character is not having

        weaponCandidates = havingWeapons.Where((Weapon weapon) => {
            return weapon.CurrentLevel < weapon.MaxLevel;
        }).ToList();
        if(havingWeapons.Count<MAX_ARTIFACTS_COUNT)
            remainingEquipments.AddRange(remainingWeapons);

        artifactCandidates = havingArtifacts.Where((Artifact artifact) => {
            return artifact.CurrentLevel < artifact.MaxLevel;
        }).ToList();
        if(havingArtifacts.Count<MAX_ARTIFACTS_COUNT)
            remainingEquipments.AddRange(remainingArtifacts);
            
        weaponCandidates.Shuffle();
        artifactCandidates.Shuffle();
        remainingEquipments.Shuffle();

        candidates.AddRange(weaponCandidates.GetRange(0, Math.Min(weaponCandidates.Count, number)));
        candidates.AddRange(artifactCandidates.GetRange(0, Math.Min(artifactCandidates.Count, number)));
        candidates.AddRange(remainingEquipments.GetRange(0, Math.Min(remainingEquipments.Count, number)));

        bonusChoises.Shuffle();
        candidates.Add(bonusChoises[0]);

        while(candidates.Count < number) { // Fill as much item as lack into the candidate list.
            candidates.Add(bonusChoises[0]);
            bonusChoises.Add(bonusChoises[0]);
            bonusChoises.RemoveAt(0);
        }
        candidates.Shuffle();
        return candidates.GetRange(0, 4).ToArray();
    }
    
    public void OnPlayerSelectItem(IPlayerGettable item) {
        if(item is Weapon) {
            int targetIndex;
            targetIndex = remainingWeapons.IndexOf(item as Weapon);
            if(targetIndex >= 0) {
                Weapon target = remainingWeapons[targetIndex];
                AddEquipmentToPlayer(target);
            } else {
                targetIndex = havingWeapons.IndexOf(item as Weapon);
                if(targetIndex >= 0 && !havingWeapons[targetIndex].IsMaxLevel)
                    havingWeapons[targetIndex].LevelUp();
            }

                
        } else if(item is Artifact) {
            int targetIndex;
            targetIndex = remainingArtifacts.IndexOf(item as Artifact);
            if(targetIndex >= 0) {
                Artifact target = remainingArtifacts[targetIndex];
                AddEquipmentToPlayer(target);
            } else {
                targetIndex = havingArtifacts.IndexOf(item as Artifact);
                if(targetIndex >= 0 && !havingArtifacts[targetIndex].IsMaxLevel)
                    havingArtifacts[targetIndex].LevelUp();
            }
        } else {
            item.OnGotten();
        }
    }

    public void AddEquipmentToPlayer(Equipment equipment) {
        if(equipment is Weapon) {
            havingWeapons.Add((Weapon) equipment);
            remainingWeapons.Remove((Weapon) equipment);
        } else if(equipment is Artifact) {
            havingArtifacts.Add((Artifact) equipment);
            remainingArtifacts.Remove((Artifact) equipment);
        } else {
            throw new ArgumentException("Argument is neither a weapon and a artifact.");
        }

        if(equipment.CurrentLevel == 0)
            equipment.LevelUp();

        _Character.AddEquipment(equipment);
        equipment.OnGotten();

        _UiManager.UpdateWeaponList();
        _UiManager.UpdateArtifactList();
    }

    public void RemoveEquipmentFromPlayer(Equipment equipment) {
        if(equipment is Weapon) {
            havingWeapons.Remove((Weapon) equipment);
            remainingWeapons.Add((Weapon) equipment);
        } else if(equipment is Artifact) {
            havingArtifacts.Remove((Artifact) equipment);
            remainingArtifacts.Add((Artifact) equipment);
        } else {
            throw new ArgumentException("Argument is neither a weapon and a artifact.");
        }
        _Character.RemoveEquipment(equipment);
        equipment.OnTakeOff();

        _UiManager.UpdateWeaponList();
        _UiManager.UpdateArtifactList();
    }

    public void ChangeWeapon(int index, Weapon weapon) {
        if(havingWeapons.Contains(weapon))
            return;

        var prev = havingWeapons[index];
        var next = weapon;

        remainingWeapons.Add(prev);
        remainingWeapons.Remove(next);

        _Character.RemoveEquipment(prev);
        _Character.AddEquipment(next);

        prev.OnTakeOff();
        next.OnGotten();
        
        havingWeapons[index] = next;

        _UiManager.UpdateWeaponList();
    }

    public void AddBasicWeapon(Weapon basicWeapon) {
        Weapon target = remainingWeapons.Find((Weapon weapon) => {
            return basicWeapon.EquipmentType == weapon.EquipmentType;
        });
        if(target == null)
            Debug.LogWarning("Weapon that was returned by find is null.\nEqupment Manager may doesn't have that.");
        OnPlayerSelectItem(target);
    }

    public void AddBasicArtifact(Artifact basicArtifact) {
        Artifact target = remainingArtifacts.Find((Artifact artifact) => {
            return basicArtifact.EquipmentType == artifact.EquipmentType;
        });
        if(target == null)
            Debug.LogWarning("Artifact that was returned by find is null.\nEqupment Manager may doesn't have that.");
        OnPlayerSelectItem(target);
    }
}