using System.Collections.Generic;
using UnityEngine;

public class TestSceanUI : MonoBehaviour {
    [SerializeField] private Transform equipmentParent;
    private List<Weapon> weapons = new List<Weapon>();
    private List<Artifact> artifacts = new List<Artifact>();

    bool wOpend = false;
    bool aOpend = false;
    
    private void Start() {
        Equipment[] equipments = equipmentParent.GetComponentsInChildren<Equipment>(true);
        print(equipments.Length);
        foreach(var e in equipments) {
            if(e is Weapon)
                weapons.Add((Weapon)e);
            if(e is Artifact)
                artifacts.Add((Artifact)e);
        }
    }

    private void OnGUI() {
        float width = 150;
        float height = 770;
        if(GUI.Button(new Rect(10, 1070-height-30, width, 30), "Weapons"))
            wOpend = !wOpend;

        if(GUI.Button(new Rect(160, 1070-height-30, width, 30), "Artifacts"))
            aOpend = !aOpend;

        if(wOpend) {
            GUI.Box(new Rect(10, 1070 - height, width, height), "");
            for(int i=0; i<weapons.Count; i++) {
                if(GUI.Button(new Rect(10+i%2*75, 1070-height+(int)(i/2)*40, 75, 40), weapons[i].Name)) {
                    GameManager.instance.StageManager.EquipmentsManager.GivePlayerItem(weapons[i]);
                }
            }
        }
        
        if(aOpend) {
            GUI.Box(new Rect(160, 1070 - height, width, height), "");
            for(int i=0; i<artifacts.Count; i++) {
                if(GUI.Button(new Rect(160+i%2*75, 1070-height+(int)(i/2)*40, 75, 40), artifacts[i].Name)) {
                    GameManager.instance.StageManager.EquipmentsManager.GivePlayerItem(artifacts[i]);
                }
            }
        }
    }
}
