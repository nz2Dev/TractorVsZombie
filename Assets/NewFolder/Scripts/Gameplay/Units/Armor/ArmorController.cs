using System;
using System.Collections.Generic;


using UnityEngine;

public class ArmorController {

    private readonly RamEffectController ramEffect;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly AttachmentSystem attachmentSystem;
    private readonly VehicleSystem vehicleSystem;
    private readonly DropSystem dropSystem;
    private readonly PoseRegistry poseRegistry;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;
    private readonly ArmorView view;

    private int idCounter;
    private readonly Dictionary<int, ArmorModel> registry = new ();

    public ArmorController(HealthSystem healthSystem, FactionRegistry factionRegistry, AttachmentSystem attachmentSystem, VehicleSystem vehicleSystem, RamEffectController ramEffect, DropSystem dropSystem, ArmorView view, HitboxSystem hitboxSystem, EntityMapping entityMapping, PoseRegistry poseRegistry) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.attachmentSystem = attachmentSystem;
        this.vehicleSystem = vehicleSystem;
        this.ramEffect = ramEffect;
        this.dropSystem = dropSystem;
        this.view = view;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
        this.poseRegistry = poseRegistry;
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
        var model = new ArmorModel(nextId, prototype.config);
        registry[model.Id] = model;

        model.PoseId = poseRegistry.Add(prototype.position, default);
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.FactionId = factionRegistry.Add(prototype.factionPrototype);
        model.VehicleId = vehicleSystem.Create(prototype.position, prototype.vehiclePrefab, poseId: model.PoseId);
        model.HitboxId = hitboxSystem.Add(model.FactionId, model.PoseId, prototype.hitboxPrototype);
        model.AttachmentId = attachmentSystem.Add(prototype.attachmentPrototype, model.FactionId, model.PoseId);
        model.DropId = dropSystem.Add(prototype.dropPrototype, model.HealthId, model.PoseId);
        model.RamId = ramEffect.StartNew(model.FactionId, model.VehicleId, prototype.ramPrototype);

        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = model.FactionId
        });

        var alie = prototype.factionPrototype.alie;
        view.Show(nextId, prototype.position, prototype.visualsPrefab, alie, prototype.engineLoopSFX, prototype.worldSpaceUIPrefab);
        return model.Id;
    }

    private void DeleteArmor(ArmorModel model) {
        registry.Remove(model.Id);
        
        healthSystem.Remove(model.HealthId);
        vehicleSystem.Remove(model.VehicleId);
        poseRegistry.Remove(model.PoseId);
        attachmentSystem.Remove(model.AttachmentId);
        ramEffect.Remove(model.RamId);

        hitboxSystem.Remove(model.HitboxId);
        dropSystem.Remove(model.DropId);
        entityMapping.DeleteMapping(model.HitboxId);
        factionRegistry.Remove(model.FactionId);
        
        view.Hide(model.Id);
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

    private void SyncVehiclesPositions() {
        foreach (var model in registry.Values) {
            // TODO: register proxmity and raycast components
            var pose = poseRegistry.Read(model.PoseId);
            ramEffect.Forward(model.RamId, pose.position);
        }
    }

    private void UpdateView() {
        foreach (var model in registry.Values) {
            var vehicleState = vehicleSystem.ReadState(model.VehicleId);
            view.UpdatePose(model.Id, vehicleState);
            view.UpdateSound(model.Id, vehicleSystem.ReadGas(model.VehicleId));
            
            var healthState = model.HealthState;
            view.UpdateHealthBar(model.Id, vehicleState.position, healthState.health, healthState.maxHealth);
            if (healthState.damageResult.HasValue) {
                view.ShowTakeHit(model.Id);
            }
        }
    }

}
