using System;
using System.Collections.Generic;


using UnityEngine;

public class ArmorController {

    private readonly RamSystem ramSystem;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly AttachmentSystem attachmentSystem;
    private readonly VehicleSystem vehicleSystem;
    private readonly DropSystem dropSystem;
    private readonly PoseRegistry poseRegistry;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;

    private readonly ArmorPresenter presenter;

    private int idCounter;
    private readonly Dictionary<int, ArmorModel> registry = new ();

    public ArmorController(HealthSystem healthSystem, FactionRegistry factionRegistry, AttachmentSystem attachmentSystem, VehicleSystem vehicleSystem, RamSystem ramSystem, DropSystem dropSystem, ArmorPresenter presenter, HitboxSystem hitboxSystem, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.attachmentSystem = attachmentSystem;
        this.vehicleSystem = vehicleSystem;
        this.ramSystem = ramSystem;
        this.dropSystem = dropSystem;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
        this.poseRegistry = poseRegistry;
        this.presenter = presenter;
    }

    public int ArmorCount => registry.Count;
    public void WriteDeadArmorFiltered(List<int> armorIds) {
        armorIds.RemoveAll(id => !registry.ContainsKey(id));
    }

    public void Update() {
        ReadHealthOutput();
        RemoveDeadArmor();
        UpdatePresentation();
    }

    public int SpawnArmor(ArmorPrototype prototype) {
        var nextId = ++idCounter;
        var model = new ArmorModel(nextId);
        registry[model.Id] = model;

        model.PoseId = poseRegistry.Add(prototype.position, default);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.VehicleId = vehicleSystem.Create(prototype.position, prototype.vehiclePrefab, poseId: model.PoseId);
        model.HitboxId = hitboxSystem.Add(model.FactionId, model.PoseId, prototype.hitboxPrototype);
        model.AttachmentId = attachmentSystem.Add(prototype.attachmentPrototype, model.FactionId, model.PoseId);
        model.DropId = dropSystem.Add(prototype.dropPrototype, model.HealthId, model.PoseId);
        model.RamId = ramSystem.Add(model.FactionId, model.VehicleId, prototype.ramPrototype);

        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId
        });

        var alie = prototype.factionPrototype.alie;
        presenter.Add(nextId, prototype.position, alie, prototype.presentationPrototype);
        return model.Id;
    }

    private void DeleteArmor(ArmorModel model) {
        registry.Remove(model.Id);
        
        healthSystem.Remove(model.HealthId);
        vehicleSystem.Remove(model.VehicleId);
        poseRegistry.Remove(model.PoseId);
        attachmentSystem.Remove(model.AttachmentId);
        ramSystem.Remove(model.RamId);

        hitboxSystem.Remove(model.HitboxId);
        dropSystem.Remove(model.DropId);
        entityMapping.DeleteMapping(model.HitboxId);
        factionRegistry.Remove(model.FactionId);
        
        presenter.Remove(model.Id);
    }

    public ArmorState ReadArmorState(int armorId) {
        var armor = registry[armorId];
        return new ArmorState {
            position = poseRegistry.Read(armor.PoseId).position,
            healthId = armor.HealthId,
            factionIsAlie = factionRegistry.ReadState(armor.FactionId).alie,
            vehicleId = armor.VehicleId,
            attachmentState = attachmentSystem.ReadState(armor.AttachmentId)
        };
    }

    private void ReadHealthOutput() {
        foreach (var model in registry.Values) {
            var healthState = healthSystem.ReadState(model.HealthId);
            model.HealthState = healthState;
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

    private void UpdatePresentation() {
        foreach (var model in registry.Values) {
            var vehicleState = vehicleSystem.ReadState(model.VehicleId);
            presenter.Update(model.Id, 
                vehicleState, vehicleSystem.ReadGas(model.VehicleId), 
                model.HealthState, ramSystem.ReadState(model.RamId));
        }
    }

}
