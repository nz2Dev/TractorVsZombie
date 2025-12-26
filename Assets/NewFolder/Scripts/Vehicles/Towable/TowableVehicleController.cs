using System;
using System.Collections.Generic;

using UnityEngine;

public class TowableVehicleController {
    
    private readonly TowableVehicleView view;
    private readonly SoundManager soundManager;
    private readonly VehicleService vehicleService;
    private readonly CombatService combatService;

    private int idCounter;
    private readonly Dictionary<TowableVehicleId, TowableVehicleModel> registry = new ();

    public TowableVehicleController(TowableVehicleView view, VehicleService vehicleService, CombatService combatService, SoundManager soundManager) {
        this.view = view;
        this.vehicleService = vehicleService;
        this.combatService = combatService;
        this.soundManager = soundManager;
    }

    public void Update() {
        UpdateVehicleOrientation();
        ComputeRamDamage();
    }

    public TowableVehicleId SpawnVehicle(Vector3 position, int combatId, TowableVehicleConfig vehicleConfig) {
        var nextId = new TowableVehicleId(++idCounter);
        var model = new TowableVehicleModel(nextId, position, vehicleConfig);
        registry[nextId] = model;
        model.RamCombatId = combatId;
        model.PhysicsId = vehicleService.CreateVehicle(model.Position, model.PhysicsPrefab);
        view.AddVehicle(model.Id, model.Position, model.VisualsPrefab);
        return model.Id;
    }

    public void DeleteVehicle(TowableVehicleId id) {
        var model = registry[id];
        vehicleService.DeleteVehicle(model.PhysicsId);
        view.RemoveVehicle(model.Id);
        registry.Remove(model.Id);
    }

    public Vector3 GetVehiclePosition(TowableVehicleId id) {
        return registry[id].Position;
    }
    
    public int ReadVehiclePhysicsId(TowableVehicleId id) {
        return registry[id].PhysicsId;
    }

    public void ConnectVehicle(TowableVehicleId id, int headPhysicsId) {
        var tailModel = registry[id];
        var headState = vehicleService.GetVehicleState(headPhysicsId);
        var towardHeadRotation = Quaternion.LookRotation((headState.position - tailModel.Position).normalized, Vector3.up);
        vehicleService.UpdateVehiclePose(tailModel.PhysicsId, tailModel.Position, towardHeadRotation);
        vehicleService.MakeTowingConnection(headPhysicsId, tailModel.PhysicsId);
    }

    public void DisconnectVehicle(TowableVehicleId id) {
        var vehicle = registry[id];
        vehicleService.ClearTowingConnection(vehicle.PhysicsId);
    }

    private void UpdateVehicleOrientation() {
        foreach (var model in registry.Values) {
            model.PhysicsPose = vehicleService.GetVehicleState(model.PhysicsId);
            model.Position = model.PhysicsPose.position;
            view.UpdateVehiclePose(model.Id, model.PhysicsPose);
        }
    }

    private void ComputeRamDamage() {
        foreach (var vehicle in registry.Values) {
            if (!vehicle.RamData.enabled)
                continue;

            var affectedCount = combatService.ApplyExplosionDamage(vehicle.RamCombatId, vehicle.Position, vehicle.RamData.radius, damage: 0);
            for (int i = 0; i < affectedCount; i++) {
                var position = vehicle.Position + UnityEngine.Random.onUnitSphere * vehicle.RamData.radius;
                soundManager.PlayEffectDelayed(position, i * 0.05f, vehicle.RamData.impactSFX);
            }    
        }
    }

}