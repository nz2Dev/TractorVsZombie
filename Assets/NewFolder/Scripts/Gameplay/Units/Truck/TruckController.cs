using System;


using UnityEngine;

public class TruckController {

    private readonly TruckPresenter presenter;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly VehicleSystem vehicleSystem;
    private readonly RamSystem ramSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;

    private TruckModel model;

    public TruckController(HealthSystem healthSystem, FactionRegistry factionRegistry, RamSystem ramEffect, TruckPresenter presenter, VehicleSystem vehicleSystem, HitboxSystem hitboxSystem, EntityMapping entityMapping) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.ramSystem = ramEffect;
        this.presenter = presenter;
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
        model = new TruckModel(position == default ? prototype.position : position);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.VehicleId = vehicleSystem.Create(model.Position, prototype.vehiclePrefab, prototype.rotation);
        model.RamId = ramSystem.Add(model.FactionId, model.VehicleId, prototype.ramPrototype);
        model.HitboxId = hitboxSystem.Add(model.FactionId, model.Position, prototype.hitboxPrototype);
        
        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId,
        });

        var alie = prototype.factionPrototype.alie;
        presenter.Add(model.VehicleId.Value, model.Position, alie, prototype.presentationPrototype);
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
        ramSystem.Remove(model.RamId);
        hitboxSystem.Remove(model.HitboxId);
        
        entityMapping.DeleteMapping(model.HitboxId);
        factionRegistry.Remove(model.FactionId);

        presenter.Remove(model.VehicleId.Value);
    }

    private void WriteExternalInput() {
        ramSystem.SetPosition(model.RamId, model.Position);
        hitboxSystem.UpdatePosition(model.HitboxId, model.Position);
    }

    private void UpdateView() {
        presenter.Update(model.VehicleId.Value, 
            vehicleSystem.ReadState(model.VehicleId), 
            vehicleSystem.ReadGas(model.VehicleId), 
            model.HealthState, 
            ramSystem.ReadState(model.RamId));
    }

}
