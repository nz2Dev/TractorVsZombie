using System;


using UnityEngine;

public class TruckController {

    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly VehicleSystem vehicleSystem;
    private readonly RamSystem ramSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;
    private readonly PoseRegistry poseRegistry;

    private TruckModel model;
    private readonly TruckPresenter presenter;

    public TruckController(HealthSystem healthSystem, FactionRegistry factionRegistry, RamSystem ramEffect, TruckPresenter presenter, VehicleSystem vehicleSystem, HitboxSystem hitboxSystem, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.ramSystem = ramEffect;
        this.presenter = presenter;
        this.vehicleSystem = vehicleSystem;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
        this.poseRegistry = poseRegistry;
    }

    public virtual VehicleId ReadVehicleId() {
        return model.VehicleId;
    }

    public virtual Vector3 ReadVehiclePosition() {
        return poseRegistry.Read(model.PoseId).position;
    }

    public bool UnitExist => model != null;

    public void Update() {
        if (model == null)
            return;

        UpdatePresenter();
        CheckDestruction();
    }

    public virtual void Create(TruckPrototype prototype, Vector3 position = default) {
        var initPosition = position == default ? prototype.position : position;
        model = new TruckModel();
        model.PoseId = poseRegistry.Add(initPosition, prototype.rotation);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.VehicleId = vehicleSystem.Create(initPosition, prototype.vehiclePrototype, model.PoseId, prototype.rotation);
        model.RamId = ramSystem.Add(model.FactionId, model.VehicleId, prototype.ramPrototype);
        model.HitboxId = hitboxSystem.Add(model.FactionId, model.PoseId, prototype.hitboxPrototype);
        
        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId,
        });

        var alie = prototype.factionPrototype.alie;
        presenter.Add(model.VehicleId.Value, initPosition, alie, prototype.presentationPrototype);
    }

    private void CheckDestruction() {
        var healthState = healthSystem.ReadState(model.HealthId);
        if (healthState.isDead) {
            Clear();
            model = null;
        }
    }

    private void Clear() {
        healthSystem.Remove(model.HealthId);
        vehicleSystem.Remove(model.VehicleId);
        poseRegistry.Remove(model.PoseId);
        ramSystem.Remove(model.RamId);
        hitboxSystem.Remove(model.HitboxId);
        factionRegistry.Remove(model.FactionId);
        
        entityMapping.DeleteMapping(model.HitboxId);

        presenter.Remove(model.VehicleId.Value);
    }

    private void UpdatePresenter() {
        presenter.Update(model.VehicleId.Value, 
            vehicleSystem.ReadState(model.VehicleId), 
            vehicleSystem.ReadGas(model.VehicleId), 
            healthSystem.ReadState(model.HealthId), 
            ramSystem.ReadState(model.RamId));
    }

}
