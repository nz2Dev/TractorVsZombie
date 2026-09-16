using System;

using Combat;

using UnityEngine;

public class TruckController {

    private readonly TruckView view;
    private readonly CombatSystem combatSystem;
    private readonly VehicleService vehicleService;
    private readonly RamEffectController ramEffect;
    private readonly ProximityService proximityService;
    private readonly RaycastService raycastService;
    private readonly EntityMapping entityMapping;

    private TruckModel model;

    public TruckController(CombatSystem combatSystem, RamEffectController ramEffect, TruckView view, VehicleService vehicleService, ProximityService proximityService, RaycastService raycastService, EntityMapping entityMapping) {
        this.combatSystem = combatSystem;
        this.ramEffect = ramEffect;
        this.view = view;
        this.vehicleService = vehicleService;
        this.proximityService = proximityService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
    }

    public virtual int ReadVehiclePhysicsId() => model.VehiclePhysicsId;
    public virtual Vector3 ReadVehiclePosition() => model.Position;
    public bool UnitExist => model != null;

    public void Update() {
        if (model == null)
            return;

        ReadExternalState();
        ReadCombatState();
        WriteExternalInput();
        UpdateView();
        CheckDestruction();
    }

    public virtual void Create(TruckPrototype prototype, Vector3 position = default) {
        model = new TruckModel(prototype.config, position == default ? prototype.position : position);
        model.CombatId = combatSystem.Add(prototype.combatPrototype);
        model.VehiclePhysicsId = vehicleService.CreateVehicle(model.Position, prototype.vehiclePrefab, prototype.rotation);
        model.RamId = ramEffect.StartNew(model.CombatId, model.VehiclePhysicsId, prototype.combatPrototype.alie, prototype.ramPrototype);
        model.ProximityId = proximityService.AddPoint(model.Position, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));
        model.RaycastId = raycastService.RegisterMarker(model.Position, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));
        
        entityMapping.CreateMappings(new EntityComponents {
            proximityId = model.ProximityId,
            raycastId = model.RaycastId,
            combatId = model.CombatId,
        });

        view.Show(model.Position, prototype.visualsPrefab, prototype.combatPrototype.alie, prototype.engineLoopSFX, prototype.worldSpaceUIPrefab);
    }

    public virtual void Drive(float driveInput, bool boostInput) {
        var boostMultiplier = boostInput ? 2f : 1f;   // gameplay rule: boost doubles throttle
        model.Gas = driveInput * boostMultiplier;
    }

    public virtual void Brake(float brakes) {
        model.Brakes = brakes;
    } 

    public virtual void Steer(float steerInput) {
        model.Steer = steerInput;   // -1..1; traction limiting happens inside VehiclePhysics
    }

    private void ReadExternalState() {
        model.VehiclePhysicsState = vehicleService.GetVehicleState(model.VehiclePhysicsId);
        model.Position = model.VehiclePhysicsState.position;
    }

    private void ReadCombatState() {
        var combatState = combatSystem.ReadState(model.CombatId);
        if (combatState.health <= 0) {
            model.Destroyed = true;
        }
    }

    private void CheckDestruction() {
        if (model.Destroyed) {
            Clear();
            model = null;
        }
    }

    private void Clear() {
        combatSystem.Remove(model.CombatId);
        vehicleService.DeleteVehicle(model.VehiclePhysicsId);
        ramEffect.Remove(model.RamId);
        proximityService.RemovePoint(model.ProximityId);
        raycastService.UnregisterMarker(model.RaycastId);
        
        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);

        view.Remove();
    }

    private void WriteExternalInput() {
        ramEffect.Forward(model.RamId, model.Position);
        // todo: register proxmity and raycast components
        vehicleService.SetVehicleInput(model.VehiclePhysicsId, model.Gas, model.Brakes, model.Steer);
        proximityService.UpdatePoint(model.ProximityId, model.Position);
        raycastService.UpdateMarker(model.RaycastId, model.Position);
    }

    private void UpdateView() {
        view.UpdatePose(model.VehiclePhysicsState);
        view.UpdateSound(model.Gas);

        var combatState = combatSystem.ReadState(model.CombatId);
        view.UpdateHealthBar(model.Position, combatState.health, combatState.maxHealth);
        if (combatState.damageResult.HasValue) {
            view.ShowTakeHit();
        }
    }

}
