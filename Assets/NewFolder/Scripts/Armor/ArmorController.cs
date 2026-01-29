using System;
using System.Collections.Generic;

using UnityEngine;

public class ArmorController {

    private readonly RamEffect ramEffect;
    private readonly CombatSystem combatSystem;
    private readonly WeaponController weaponController;
    private readonly MotorVehicleController motorVehicleController;

    private int idCounter;    
    private readonly Dictionary<int, ArmorModel> registry = new ();
    private readonly List<ArmorModel> diedArmor = new ();

    public ArmorController(CombatSystem combatSystem, WeaponController weaponController, MotorVehicleController motorVehicleController, RamEffect ramEffect) {
        this.combatSystem = combatSystem;
        this.weaponController = weaponController;
        this.motorVehicleController = motorVehicleController;
        this.ramEffect = ramEffect;
    }

    public int ArmorCount => registry.Count;
    public IReadOnlyList<ArmorModel> DiedArmor => diedArmor;
    public void ClearDiedRegistry() => diedArmor.Clear();

    public void Update() {
        ReadVehiclesCombat();
        RemoveDeadArmor();
        SyncVehiclesPositions();
    }

    public int SpawnArmor(Vector3 position, ArmorConfig armorConfig) {
        var nextId = ++idCounter;
        var model = new ArmorModel(nextId, position, armorConfig);
        registry[model.Id] = model;
        model.CombatId = combatSystem.RegisterAgent(position, alie: false);
        model.VehicleId = motorVehicleController.SpawnVehicle(position, model.VehicleConfig);
        model.WeaponId = weaponController.SpawnWeapon(model.CombatId, position, model.WeaponConfig);
        model.RamId = ramEffect.StartNew(model.Position, model.CombatId, model.RamConfig);
        model.Health = model.MaxHealth;
        return model.Id;
    }

    private void DeleteArmor(ArmorModel model) {
        motorVehicleController.DeleteVehicle(model.VehicleId);
        combatSystem.UnregisterAgent(model.CombatId);
        weaponController.DeleteWeapon(model.WeaponId);
        ramEffect.Stop(model.RamId);
        registry.Remove(model.Id);
    }

    public void WriteDeadArmorFiltered(List<int> armorIds) {
        armorIds.RemoveAll(id => !registry.ContainsKey(id));
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

    private void ReadVehiclesCombat() {
        foreach (var model in registry.Values) {
            var combatState = combatSystem.GetAgentState(model.CombatId);
            
            if (combatState.projectiled || combatState.exploded) {    
                model.Health -= combatState.damage;
            }

            combatSystem.ClearAgentState(model.CombatId);

            if (model.Health <= 0) {
                diedArmor.Add(model);
            }
        }
    }

    private void RemoveDeadArmor() {
        List<ArmorModel> removeBuffer = new();
        foreach (var model in registry.Values) {
            if (model.Health <= 0) {
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