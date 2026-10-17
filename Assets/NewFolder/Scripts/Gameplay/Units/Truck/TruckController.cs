using System;

using Combat;

using UnityEngine;

public class TruckController {

    private readonly TruckView view;
    private readonly CombatSystem combatSystem;
    private readonly VehicleSystem vehicleSystem;
    private readonly RamEffectController ramEffect;
    private readonly ProximityService proximityService;
    private readonly RaycastService raycastService;
    private readonly EntityMapping entityMapping;

    private TruckModel model;

    public TruckController(CombatSystem combatSystem, RamEffectController ramEffect, TruckView view, VehicleSystem vehicleSystem, ProximityService proximityService, RaycastService raycastService, EntityMapping entityMapping) {
        this.combatSystem = combatSystem;
        this.ramEffect = ramEffect;
        this.view = view;
        this.vehicleSystem = vehicleSystem;
        this.proximityService = proximityService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
    }

    public virtual VehicleId ReadVehicleId() => model.VehicleId;
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
        model.VehicleId = vehicleSystem.Create(model.Position, prototype.vehiclePrefab, prototype.rotation);
        model.RamId = ramEffect.StartNew(model.CombatId, model.VehicleId, prototype.combatPrototype.alie, prototype.ramPrototype);
        model.ProximityId = proximityService.AddPoint(model.Position, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));
        model.RaycastId = raycastService.RegisterMarker(model.Position, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));
        
        entityMapping.CreateMappings(new EntityComponents {
            proximityId = model.ProximityId,
            raycastId = model.RaycastId,
            combatId = model.CombatId,
        });

        view.Show(model.Position, prototype.visualsPrefab, prototype.combatPrototype.alie, prototype.engineLoopSFX, prototype.worldSpaceUIPrefab);
    }

    private void ReadExternalState() {
        model.Position = vehicleSystem.ReadState(model.VehicleId).position;
    }

    private void ReadCombatState() {
        model.CombatState = combatSystem.ReadState(model.CombatId);
    }

    private void CheckDestruction() {
        if (model.CombatState.isDead) {
            Clear();
            model = null;
        }
    }

    private void Clear() {
        combatSystem.Remove(model.CombatId);
        vehicleSystem.Remove(model.VehicleId);
        ramEffect.Remove(model.RamId);
        proximityService.RemovePoint(model.ProximityId);
        raycastService.UnregisterMarker(model.RaycastId);
        
        entityMapping.DeleteMappings(model.ProximityId, model.RaycastId);

        view.Remove();
    }

    private void WriteExternalInput() {
        ramEffect.Forward(model.RamId, model.Position);
        // todo: register proxmity and raycast components
        proximityService.UpdatePoint(model.ProximityId, model.Position);
        raycastService.UpdateMarker(model.RaycastId, model.Position);
    }

    private void UpdateView() {
        view.UpdatePose(vehicleSystem.ReadState(model.VehicleId));
        view.UpdateSound(vehicleSystem.ReadGas(model.VehicleId));

        var combatState = model.CombatState;
        view.UpdateHealthBar(model.Position, combatState.health, combatState.maxHealth);
        if (combatState.damageResult.HasValue) {
            view.ShowTakeHit();
        }
    }

}
