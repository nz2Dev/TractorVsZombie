using System;


using UnityEngine;

public class TruckController {

    private readonly TruckView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly VehicleSystem vehicleSystem;
    private readonly RamEffectController ramEffect;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;

    private TruckModel model;

    public TruckController(HealthSystem healthSystem, FactionRegistry factionRegistry, RamEffectController ramEffect, TruckView view, VehicleSystem vehicleSystem, HitboxSystem hitboxSystem, EntityMapping entityMapping) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.ramEffect = ramEffect;
        this.view = view;
        this.vehicleSystem = vehicleSystem;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
    }

    public virtual VehicleId ReadVehicleId() => model.VehicleId;
    public virtual Vector3 ReadVehiclePosition() => model.Position;
    public bool UnitExist => model != null;

    public void Update() {
        if (model == null)
            return;

        ReadExternalState();
        ReadHealthState();
        WriteExternalInput();
        UpdateView();
        CheckDestruction();
    }

    public virtual void Create(TruckPrototype prototype, Vector3 position = default) {
        model = new TruckModel(prototype.config, position == default ? prototype.position : position);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        var alie = factionRegistry.ReadState(model.FactionId).alie;
        model.VehicleId = vehicleSystem.Create(model.Position, prototype.vehiclePrefab, prototype.rotation);
        model.RamId = ramEffect.StartNew(model.FactionId, model.VehicleId, prototype.ramPrototype);
        model.HitboxId = hitboxSystem.Add(model.FactionId, model.Position, prototype.raycastMarkerPrefab);
        
        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId,
        });

        view.Show(model.Position, prototype.visualsPrefab, alie, prototype.engineLoopSFX, prototype.worldSpaceUIPrefab);
    }

    private void ReadExternalState() {
        model.Position = vehicleSystem.ReadState(model.VehicleId).position;
    }

    private void ReadHealthState() {
        model.HealthState = healthSystem.ReadState(model.HealthId);
    }

    private void CheckDestruction() {
        if (model.HealthState.isDead) {
            Clear();
            model = null;
        }
    }

    private void Clear() {
        healthSystem.Remove(model.HealthId);
        vehicleSystem.Remove(model.VehicleId);
        ramEffect.Remove(model.RamId);
        hitboxSystem.Remove(model.HitboxId);
        
        entityMapping.DeleteMapping(model.HitboxId);
        factionRegistry.Remove(model.FactionId);

        view.Remove();
    }

    private void WriteExternalInput() {
        ramEffect.Forward(model.RamId, model.Position);
        hitboxSystem.UpdatePosition(model.HitboxId, model.Position);
    }

    private void UpdateView() {
        view.UpdatePose(vehicleSystem.ReadState(model.VehicleId));
        view.UpdateSound(vehicleSystem.ReadGas(model.VehicleId));

        var healthState = model.HealthState;
        view.UpdateHealthBar(model.Position, healthState.health, healthState.maxHealth);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit();
        }
    }

}
