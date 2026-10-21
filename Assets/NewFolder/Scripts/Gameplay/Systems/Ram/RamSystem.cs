using System.Collections.Generic;

using Interactions;
using UnityEngine;

public class RamSystem {
    
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly HitboxSystem hitboxSystem;
    private readonly VehicleSystem vehicleSystem;
    private readonly MotionSystem motionSystem;
    private readonly EntityMapping entityMapping;
    
    private int idCounter;
    private readonly Dictionary<RamId, RamModel> registry = new();

    public RamSystem(HealthSystem healthSystem, FactionRegistry factionRegistry, HitboxSystem hitboxSystem, MotionSystem motionSystem, EntityMapping entityMapping, VehicleSystem vehicleSystem) {
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.hitboxSystem = hitboxSystem;
        this.motionSystem = motionSystem;
        this.entityMapping = entityMapping;
        this.vehicleSystem = vehicleSystem;
    }

    public void Update() {
        DecayTemporalLinearDrag();
        ComputeDamage();
        ApplyTemporalLinearDrag();
    }

    public RamId Add(FactionId holderFactionId, VehicleId holderVehicleId, RamPrototype prototype) {
        var id = new RamId(idCounter++);
        var baseLinearDumping = vehicleSystem.GetLinearDamping(holderVehicleId);
        var model = new RamModel(id, prototype.config, holderFactionId, holderVehicleId, baseLinearDumping);
        registry[id] = model;
        return id;
    }

    public void Remove(RamId ramId) {
        registry.Remove(ramId, out var model);
        if (vehicleSystem.Exists(model.HolderVehicleId)) {
            vehicleSystem.SetLinearDamping(model.HolderVehicleId, model.BaseLinearDrag);
        }
    }

    public RamState ReadState(RamId ramId) => new() { 
        receivedContacts = registry[ramId].ReceivedContacts 
    };

    private void ComputeDamage() {
        foreach (var model in registry.Values) {
            var holderIsAlly = factionRegistry.ReadState(model.HolderFactionId).alie;
            var vehicleState = vehicleSystem.ReadPhysicsPose(model.HolderVehicleId);
            hitboxSystem.Overlap(vehicleState.position, model.Config.triggerRadius, !holderIsAlly, out var overlapIds);
            
            model.LostContactBuffer.Clear();
            model.LostContactBuffer.AddRange(model.InContact);
            model.ReceiveContactBuffer.Clear();
            foreach (var id in overlapIds) {
                if (model.InContact.Contains(id)) {
                    model.LostContactBuffer.Remove(id);
                } else {
                    model.ReceiveContactBuffer.Add(id);  
                } 
            }

            model.InContact.AddRange(model.ReceiveContactBuffer);
            foreach (var id in model.LostContactBuffer) {
                model.InContact.Remove(id);
            }
            
            model.ReceivedContacts = model.ReceiveContactBuffer.Count;
            if (model.ReceivedContacts > 0) {
                var config = model.Config;
                model.TemporalLinearDrag = Mathf.Min(
                    model.TemporalLinearDrag + model.ReceivedContacts * config.temporalLinearDragPerContact, 
                    config.maxTemporalLinearDrag);
            }

            entityMapping.FindByHitboxIds(model.ReceiveContactBuffer, out var contactComponents);
            var speed = vehicleState.velocity.magnitude;
            if (speed < model.Config.minImpactSpeed) 
                continue;

            foreach (var components in contactComponents) {
                if (components.motionId.HasValue) {
                    var explosionData = model.Config.explosionData;
                    explosionData.force *= Mathf.Clamp01(speed / model.Config.maxImpactSpeed);
                    motionSystem.AddExplosionEffect(components.motionId.Value, new Explosion { 
                        epicentr = vehicleState.position, 
                        config = explosionData 
                    });
                }

                if (components.healthId.HasValue) {
                    healthSystem.DealDamage(components.healthId.Value, new DamageInput { 
                        damageSource = vehicleState.position, 
                        damageType = DamageType.Exposion, 
                        damage = model.Config.damage 
                    });
                }
            }

            // evidence that Ram as a System do not fit or violate single responsibility:
            // - its not a part of entity, its more of a side effect or capability, 
            //   like weapon wich is separate entity.
            // - explosion of contacts is one responsibility, 
            //   but the slow down and vehicle affects is another
            // evidence for a system
            // - it affects vehicle and rely on vehicle state
        }
    }

    private void DecayTemporalLinearDrag() {
        foreach (var model in registry.Values)
            model.TemporalLinearDrag = Mathf.MoveTowards(model.TemporalLinearDrag, 0, model.Config.temporalLinearDragRecoverySpeed * Time.deltaTime);
    }

    private void ApplyTemporalLinearDrag() {
        foreach (var model in registry.Values)
            vehicleSystem.SetLinearDamping(model.HolderVehicleId, model.BaseLinearDrag + model.TemporalLinearDrag);
    }
}
