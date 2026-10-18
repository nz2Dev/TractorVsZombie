using System;
using System.Collections.Generic;


using UnityEngine;

public class ArmorController {

    private readonly RamEffectController ramEffect;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly WeaponController weaponController;
    private readonly VehicleSystem vehicleSystem;
    private readonly RewardController rewardController;
    private readonly ProximityService proximityService;
    private readonly RaycastService raycastService;
    private readonly EntityMapping entityMapping;
    private readonly ArmorView view;

    private int idCounter;
    private readonly Dictionary<int, ArmorModel> registry = new ();

    public ArmorController(HealthSystem healthSystem, FactionRegistry factionRegistry, WeaponController weaponController, VehicleSystem vehicleSystem, RamEffectController ramEffect, RewardController rewardController, ArmorView view, ProximityService proximityService, RaycastService raycastService, EntityMapping entityMapping) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.weaponController = weaponController;
        this.vehicleSystem = vehicleSystem;
        this.ramEffect = ramEffect;
        this.rewardController = rewardController;
        this.view = view;
        this.proximityService = proximityService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
    }

    public int ArmorCount => registry.Count;
    public void WriteDeadArmorFiltered(List<int> armorIds) {
        armorIds.RemoveAll(id => !registry.ContainsKey(id));
    }

    public void Update() {
        ReadHealthOutput();
        RemoveDeadArmor();

        SyncVehiclesPositions();
        UpdateView();
    }

    public int SpawnArmor(ArmorPrototype prototype) {
        var nextId = ++idCounter;
        var model = new ArmorModel(nextId, prototype.position, prototype.config, prototype.rewardPrototype, prototype.localWeaponPrototype.position);
        registry[model.Id] = model;

        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        var alie = factionRegistry.ReadState(model.FactionId).alie;
        model.VehicleId = vehicleSystem.Create(prototype.position, prototype.vehiclePrefab);
        model.ProximityId = proximityService.AddPoint(prototype.position, FactionRegistry.GetProximityLayerForFaction(factionRegistry.ReadState(model.FactionId).alie));
        model.RaycastId = raycastService.RegisterMarker(prototype.position, prototype.raycastMarkerPrefab, FactionRegistry.GetRaycastLayerForFaction(factionRegistry.ReadState(model.FactionId).alie));

        model.RamId = ramEffect.StartNew(model.FactionId, model.VehicleId, prototype.ramPrototype);
        model.WeaponId = weaponController.SpawnWeapon(model.FactionId, prototype.localWeaponPrototype);

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = model.ProximityId,
            raycastId = model.RaycastId,
            healthId = model.HealthId,
            factionId = model.FactionId
        });

        view.Show(nextId, prototype.position, prototype.visualsPrefab, alie, prototype.engineLoopSFX, prototype.worldSpaceUIPrefab);
        return model.Id;
    }

    private void DeleteArmor(ArmorModel model) {
        registry.Remove(model.Id);
        
        healthSystem.Remove(model.HealthId);
        vehicleSystem.Remove(model.VehicleId);
        weaponController.DeleteWeapon(model.WeaponId);
        ramEffect.Remove(model.RamId);

        proximityService.RemovePoint(model.ProximityId);
        raycastService.UnregisterMarker(model.RaycastId);
        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);
        factionRegistry.Remove(model.FactionId);
        
        view.Hide(model.Id);
    }

    public ArmorState ReadArmorState(int armorId) {
        var armor = registry[armorId];
        return new ArmorState {
            position = armor.Position,
            healthId = armor.HealthId,
            factionIsAlie = factionRegistry.ReadState(armor.FactionId).alie,
            vehicleId = armor.VehicleId,
            weaponId = armor.WeaponId,
            weaponState = weaponController.ReadWeaponState(armor.WeaponId)
        };
    }

    private void ReadHealthOutput() {
        foreach (var model in registry.Values) {
            var healthState = healthSystem.ReadState(model.HealthId);
            model.HealthState = healthState;
            if (healthState.damageResult?.damageWasFatal == true) {
                rewardController.Create(model.RewardPrototype, model.Position, vehicleSystem.ReadState(model.VehicleId).rotation);
            }
        }
    }

    private void RemoveDeadArmor() {
        List<ArmorModel> removeBuffer = new();
        foreach (var model in registry.Values) {
            if (model.HealthState.isDead) {
                removeBuffer.Add(model);
            }
        }
        foreach (var model in removeBuffer) {
            DeleteArmor(model);
        }
    }

    private void SyncVehiclesPositions() {
        foreach (var model in registry.Values) {
            var vehicleState = vehicleSystem.ReadState(model.VehicleId);
            model.Position = vehicleState.position;

            weaponController.MoveWeapon(model.WeaponId, model.Position + model.WeaponPlacementOffset);
            // healthSystem.UpdateAgentPosition(model.HealthId, model.Position);
            // TODO: register proxmity and raycast components
            ramEffect.Forward(model.RamId, model.Position);
            proximityService.UpdatePoint(model.ProximityId, model.Position);
            raycastService.UpdateMarker(model.RaycastId, model.Position);
        }
    }

    private void UpdateView() {
        foreach (var model in registry.Values) {
            var vehicleState = vehicleSystem.ReadState(model.VehicleId);
            view.UpdatePose(model.Id, vehicleState);
            view.UpdateSound(model.Id, vehicleSystem.ReadGas(model.VehicleId));
            
            var healthState = model.HealthState;
            view.UpdateHealthBar(model.Id, model.Position, healthState.health, healthState.maxHealth);
            if (healthState.damageResult.HasValue) {
                view.ShowTakeHit(model.Id);
            }
        }
    }

}
