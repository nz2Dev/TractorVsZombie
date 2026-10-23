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
        ComputeDamage();
    }

    public RamId Add(FactionId holderFactionId, VehicleId holderVehicleId, RamPrototype prototype) {
        var id = new RamId(idCounter++);
        var model = new RamModel(id, prototype.config, holderFactionId, holderVehicleId);
        registry[id] = model;
        return id;
    }

    public void Remove(RamId ramId) {
        registry.Remove(ramId);
    }

    public RamState ReadState(RamId ramId) => new() { 
        receivedContacts = registry[ramId].ReceivedContacts 
    };

    private void ComputeDamage() {
        foreach (var model in registry.Values) {
            var holderIsAlly = factionRegistry.ReadState(model.HolderFactionId).alie;
            var vehicleState = vehicleSystem.ReadState(model.HolderVehicleId);
            hitboxSystem.Overlap(vehicleState.pose.position, model.Config.triggerRadius, !holderIsAlly, out var overlapIds);
            
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
                vehicleSystem.AddDragImpulse(
                    model.HolderVehicleId,
                    model.ReceivedContacts * config.dragImpulsePerContact);
            }

            entityMapping.FindByHitboxIds(model.ReceiveContactBuffer, out var contactComponents);
            var speed = vehicleState.pose.velocity.magnitude;
            if (speed < model.Config.minImpactSpeed) 
                continue;

            foreach (var components in contactComponents) {
                if (components.motionId.HasValue) {
                    var explosionData = model.Config.explosionData;
                    explosionData.force *= Mathf.Clamp01(speed / model.Config.maxImpactSpeed);
                    motionSystem.AddExplosionEffect(components.motionId.Value, new Explosion { 
                        epicentr = vehicleState.pose.position, 
                        config = explosionData 
                    });
                }

                if (components.healthId.HasValue) {
                    healthSystem.DealDamage(components.healthId.Value, new DamageInput { 
                        damageSource = vehicleState.pose.position, 
                        damageType = DamageType.Exposion, 
                        damage = model.Config.damage 
                    });
                }
            }
        }
    }

}
