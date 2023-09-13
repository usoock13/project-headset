using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentsManager : MonoBehaviour {
    [SerializeField] private List<Equipment> allEquipments;
    [SerializeField] private List<Equipment> playerHas;
    [SerializeField] private List<Equipment> playerHasNot;

    private void Awake() {
        allEquipments = transform.GetComponentsInChildren<Equipment>(true).ToList();
    }
    public Equipment[] GetNextChoises(int number) {
        throw new System.NotImplementedException();
    }
}