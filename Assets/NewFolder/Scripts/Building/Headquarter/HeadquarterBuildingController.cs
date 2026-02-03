
using System;
using System.Collections.Generic;

using UnityEngine;

public class HeadquarterBuildingController {
    
    private readonly CombatSystem combatSystem;

    private GameObject visuals;
    private HeadquarterBuilding headquarter;

    public HeadquarterBuildingController(CombatSystem combatSystem) {
        this.combatSystem = combatSystem;
    }

    public void Update() {
        ReadCombatOutput();
        CheckLooseCondition();
    }

    public void SetHeadquearter(Vector3 position, Quaternion rotation, HeadquarterBuildingConfig config) {
        headquarter = new HeadquarterBuilding(config);
        headquarter.Position = position;
        headquarter.CombatId = combatSystem.RegisterAgent(position, alie: true, config.maxHealth, height: 2);
        visuals = GameObject.Instantiate(config.visualsPrefab, position, rotation);
    }

    private void ReadCombatOutput() {
        var combatOutput = combatSystem.GetCombatOutput(headquarter.CombatId);
        if (combatOutput.damageWasFatal) {
            headquarter.Destroyed = true;
            GameObject.Destroy(visuals);
        }
    }

    private void CheckLooseCondition() {
        if (headquarter.Destroyed) {
            Debug.Log("Game over");
        }
    }

}