using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class EquipmentManager : MonoBehaviour {
    Character Character => GameManager.instance.StageManager.Character;

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
    [SerializeField] private Item[] bonusChoises;

    [SerializeField] private Artifact __testItem;

    private void Awake() {
        remainingWeapons = transform.GetComponentsInChildren<Weapon>(true).ToList();
        remainingArtifacts = transform.GetComponentsInChildren<Artifact>(true).ToList();
        havingWeapons = new List<Weapon>();
        havingArtifacts = new List<Artifact>();

        bonusChoises = bonusChoisesParent.GetComponentsInChildren<Item>(true);
    }
    #if UNITY_EDITOR
    private void Update() {
        if(Input.GetKeyDown(KeyCode.G))
            GivePlayerItem(__testItem);
    }
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
            
        var rand = new System.Random();
        weaponCandidates.Shuffle();
        artifactCandidates.Shuffle();
        remainingEquipments.Shuffle();

        candidates.AddRange(weaponCandidates.GetRange(0, Math.Min(weaponCandidates.Count, number)));
        candidates.AddRange(artifactCandidates.GetRange(0, Math.Min(artifactCandidates.Count, number)));
        candidates.AddRange(remainingEquipments.GetRange(0, Math.Min(remainingEquipments.Count, number)));

        while(candidates.Count < number) {
            candidates.Add(bonusChoises[UnityEngine.Random.Range(0, bonusChoises.Length)]);
        }
        candidates.Shuffle();
        return candidates.GetRange(0, 4).ToArray();
    }
    
    public void GivePlayerItem(IPlayerGettable item) {
        if(item is Weapon) {
            int targetIndex = remainingWeapons.IndexOf(item as Weapon);
            if(targetIndex >= 0) {
                Weapon target = remainingWeapons[targetIndex];
                remainingWeapons.Remove(target);
                havingWeapons.Add(target);
                Character.AddWeapon(target);
                item.OnGotten();
            }
            targetIndex = havingWeapons.IndexOf(item as Weapon);
            if(targetIndex >= 0)
                havingWeapons[targetIndex].LevelUp();
        } else if(item is Artifact) {
            int targetIndex = remainingArtifacts.IndexOf(item as Artifact);
            if(targetIndex >= 0) {
                Artifact target = remainingArtifacts[targetIndex];
                remainingArtifacts.Remove(target);
                havingArtifacts.Add(target);
                Character.AddArtifact(target);
                item.OnGotten();
            }
            targetIndex = havingArtifacts.IndexOf(item as Artifact);
            if(targetIndex >= 0)
                havingArtifacts[targetIndex].LevelUp();
        } else {
            item.OnGotten();
        }
    }
    public void AddBasicWeapon(Weapon basicWeapon) {
        Weapon target = remainingWeapons.Find((Weapon weapon) => {
            return basicWeapon.EquipmentType == weapon.EquipmentType;
        });
        if(target == null)
            Debug.LogWarning("Weapon that was returned by find is null.\nEqupment Manager may doesn't have that.");
        GivePlayerItem(target);
    }
    public void AddBasicArtifact(Artifact basicArtifact) {
        Artifact target = remainingArtifacts.Find((Artifact artifact) => {
            return basicArtifact.EquipmentType == artifact.EquipmentType;
        });
        if(target == null)
            Debug.LogWarning("Artifact that was returned by find is null.\nEqupment Manager may doesn't have that.");
        GivePlayerItem(target);
    }
}