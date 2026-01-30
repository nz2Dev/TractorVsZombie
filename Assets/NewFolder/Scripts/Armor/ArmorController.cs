using System;
using System.Collections.Generic;

using UnityEngine;

public class ArmorController {

    private readonly RamEffect ramEffect;
    private readonly CombatSystem combatSystem;
    private readonly WeaponController weaponController;
    private readonly MotorVehicleController motorVehicleController;
    private readonly RewardController rewardController;

    private int idCounter;    
    private readonly Dictionary<int, ArmorModel> registry = new ();

    public ArmorController(CombatSystem combatSystem, WeaponController weaponController, MotorVehicleController motorVehicleController, RamEffect ramEffect, RewardController rewardController) {
        this.combatSystem = combatSystem;
        this.weaponController = weaponController;
        this.motorVehicleController = motorVehicleController;
        this.ramEffect = ramEffect;
        this.rewardController = rewardController;
    }

    public int ArmorCount => registry.Count;
    public void WriteDeadArmorFiltered(List<int> armorIds) {
        armorIds.RemoveAll(id => !registry.ContainsKey(id));
    }

    public void Update() {
        ReadCombatOutput();
        RemoveDeadArmor();
        SyncVehiclesPositions();
    }

    public int SpawnArmor(Vector3 position, ArmorConfig armorConfig) {
        var nextId = ++idCounter;
        var model = new ArmorModel(nextId, position, armorConfig);
        registry[model.Id] = model;
        model.CombatId = combatSystem.RegisterAgent(position, alie: false, model.MaxHealthConfig);
        model.VehicleId = motorVehicleController.SpawnVehicle(position, model.VehicleConfig);
        model.WeaponId = weaponController.SpawnWeapon(model.CombatId, position, model.WeaponConfig);
        model.RamId = ramEffect.StartNew(model.Position, model.CombatId, model.RamConfig);
        return model.Id;
    }

    private void DeleteArmor(ArmorModel model) {
        motorVehicleController.DeleteVehicle(model.VehicleId);
        combatSystem.UnregisterAgent(model.CombatId);
        weaponController.DeleteWeapon(model.WeaponId);
        ramEffect.Stop(model.RamId);
        registry.Remove(model.Id);
    }
    
    public ArmorState ReadArmorState(int armorId) {
        var armor = registry[armorId];
        return new ArmorState {
            position = armor.Position,
            combatId = armor.CombatId,
            vehicleId = armor.VehicleId,
            weaponId = armor.WeaponId,
            weaponConfig = armor.WeaponConfig
        };
    }

    private void ReadCombatOutput() {
        foreach (var model in registry.Values) {
            var combatOutput = combatSystem.GetCombatOutput(model.CombatId);
            if (combatOutput.damageWasFatal) {
                model.Destroyed = true;
                rewardController.SpawnWeaponReward(model.Position, model.WeaponConfig);
            }
        }
    }

    private void RemoveDeadArmor() {
        List<ArmorModel> removeBuffer = new();
        foreach (var model in registry.Values) {
            if (model.Destroyed) {
                removeBuffer.Add(model);
            }
        }
        foreach (var model in removeBuffer) {
            DeleteArmor(model);
        }
    }

    private void SyncVehiclesPositions() {
        foreach (var model in registry.Values) {
            model.Position = motorVehicleController.GetVehiclePosition(model.VehicleId);
            weaponController.MoveWeapon(model.WeaponId, model.Position);
            combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
            ramEffect.Forward(model.RamId, model.Position);
        }
    }

}