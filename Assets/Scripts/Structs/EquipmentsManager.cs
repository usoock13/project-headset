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

    [SerializeField] private Equipment bonusItem;

    private void Awake() {
        remainingWeapon = transform.GetComponentsInChildren<Weapon>(true).ToList();
        remainingArtifact = transform.GetComponentsInChildren<Artifact>(true).ToList();
        havingWeapon = new List<Weapon>();
        havingArtifact = new List<Artifact>();
    }
    public Equipment[] RandomChoises(int number) {
        List<Equipment> result = new List<Equipment>();

        if(havingWeapon.Any()) {
            result.Add(havingWeapon[UnityEngine.Random.Range(0, havingWeapon.Count)]);
        }
        if(havingArtifact.Any() && result.Count < number) {
            result.Add(havingArtifact[UnityEngine.Random.Range(0, havingArtifact.Count)]);
        }
        List<Equipment> allEquipments = (List<Equipment>) remainingWeapon.Concat<Equipment>(remainingArtifact);
        while(result.Count < number) {
            allEquipments.Sort((a, b) => {
                return UnityEngine.Random.Range(-1, 1)<0 ? -1 : 1;
            });
            result.Add(allEquipments[UnityEngine.Random.Range(0, allEquipments.Count)]);
        }
        return result.ToArray();
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