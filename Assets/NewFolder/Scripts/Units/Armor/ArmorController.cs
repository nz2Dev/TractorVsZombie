using System;
using System.Collections.Generic;

using UnityEngine;

public class ArmorController {

    private readonly RamEffectController ramEffect;
    private readonly CombatSystem combatSystem;
    private readonly WeaponController weaponController;
    private readonly VehicleService vehicleService;
    private readonly RewardController rewardController;
    private readonly ArmorView view;

    private int idCounter;    
    private readonly Dictionary<int, ArmorModel> registry = new ();

    public ArmorController(CombatSystem combatSystem, WeaponController weaponController, VehicleService vehicleService, RamEffectController ramEffect, RewardController rewardController, ArmorView view) {
        this.combatSystem = combatSystem;
        this.weaponController = weaponController;
        this.vehicleService = vehicleService;
        this.ramEffect = ramEffect;
        this.rewardController = rewardController;
        this.view = view;
    }

    public int ArmorCount => registry.Count;
    public void WriteDeadArmorFiltered(List<int> armorIds) {
        armorIds.RemoveAll(id => !registry.ContainsKey(id));
    }

    public void Update() {
        ReadCombatOutput();
        RemoveDeadArmor();
        
        SyncVehiclesPositions();
        UpdateVehiclePhysics();
        UpdateView();
    }

    public int SpawnArmor(ArmorPrototype prototype) {
        var nextId = ++idCounter;
        var model = new ArmorModel(nextId, prototype.position, prototype.config);
        registry[model.Id] = model;
        
        model.CombatId = combatSystem.RegisterAgent(prototype.position, prototype.config.combatConfig);
        model.VehiclePhysicsId = vehicleService.CreateVehicle(prototype.position, prototype.physicsPrefab);
        model.WeaponId = weaponController.SpawnWeapon(model.CombatId, prototype.weaponPrototype);
        model.RamId = ramEffect.StartNew(model.CombatId, prototype.ramPrototype);
        view.Show(nextId, prototype.position, prototype.visualsPrefab, prototype.engineLoopSFX);
        
        model.WeaponPlacementOffset = prototype.weaponPlacementOffset;
        
        return model.Id;
    }

    public void Drive(int armorId, float gasInput, bool boostInput) {
        var model = registry[armorId];
        model.MotorTorque = VehicleDriving.GasThrottle(gasInput, boostInput, model.DrivingConfig.maxEngineTorque);   
    }

    public void Brake(int armorId, float brakeInput) {
        var model = registry[armorId];
        model.BrakesTorque = brakeInput * model.DrivingConfig.maxBrakesTorque;
    }

    public void SteerToward(int armorId, Vector3 direction) {
        var model = registry[armorId];
        model.SteeringDegrees = VehicleDriving.SteerToward(direction, model.VehiclePhysicsState.velocity, model.DrivingConfig.maxSteerDegrees);
    }

    private void DeleteArmor(ArmorModel model) {
        vehicleService.DeleteVehicle(model.VehiclePhysicsId);
        combatSystem.UnregisterAgent(model.CombatId);
        weaponController.DeleteWeapon(model.WeaponId);
        ramEffect.Stop(model.RamId);
        view.Hide(model.Id);
        registry.Remove(model.Id);
    }
    
    public ArmorState ReadArmorState(int armorId) {
        var armor = registry[armorId];
        return new ArmorState {
            position = armor.Position,
            combatId = armor.CombatId,
            vehiclePhysicsId = armor.VehiclePhysicsId, // Kept as 'vehicleId' in state struct for compatibility 
            weaponId = armor.WeaponId,
            weaponConfig = armor.WeaponConfig
        };
    }

    private void ReadCombatOutput() {
        foreach (var model in registry.Values) {
            var combatOutput = combatSystem.GetCombatOutput(model.CombatId);
            if (combatOutput.damageWasFatal) {
                model.Destroyed = true;
                rewardController.SpawnLoadoutReward(model.Position, model.Config.loadoutConfig);
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
            model.VehiclePhysicsState = vehicleService.GetVehicleState(model.VehiclePhysicsId);
            model.Position = model.VehiclePhysicsState.position;
            
            weaponController.MoveWeapon(model.WeaponId, model.Position + model.WeaponPlacementOffset);
            combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
            ramEffect.Forward(model.RamId, model.Position);
        }
    }

    private void UpdateVehiclePhysics() {
        foreach (var model in registry.Values) {
            vehicleService.SetVehicleInput(model.VehiclePhysicsId, model.MotorTorque, model.BrakesTorque, model.SteeringDegrees);
        }
    }

    private void UpdateView() {
        foreach (var model in registry.Values) {
            view.UpdatePose(model.Id, model.VehiclePhysicsState);
            
            var motorRev = model.MotorTorque / Mathf.Max(model.DrivingConfig.maxEngineTorque, 1);
            view.UpdateSound(model.Id, motorRev);
        }
    }

}