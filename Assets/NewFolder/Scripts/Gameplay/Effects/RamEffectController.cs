using System;
using System.Collections.Generic;


using Interactions;

using UnityEngine;

public class RamEffectController {

    private readonly RamEffectView view;
    private readonly HealthSystem healthSystem;
    private readonly FactionRegistry factionRegistry;
    private readonly RaycastService raycastService;
    private readonly VehicleSystem vehicleSystem;
    private readonly MotionSystem motionSystem;
    private readonly EntityMapping entityMapping;

    public RamEffectController(RamEffectView view, HealthSystem healthSystem, FactionRegistry factionRegistry, RaycastService raycastService, MotionSystem motionSystem, EntityMapping entityMapping, VehicleSystem vehicleSystem) {
        this.view = view;
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.raycastService = raycastService;
        this.motionSystem = motionSystem;
        this.entityMapping = entityMapping;
        this.vehicleSystem = vehicleSystem;
    }

    private int idCounter;
    private readonly Dictionary<int, RamEffectModel> registry = new ();

    public void Update() {
        DecayTemporalLinearDrag();
        ComputeDamage();
        ApplyTemporalLinearDrag();
    }

    public int StartNew(FactionId holderFactionId, VehicleId holderVehicleId, RamEffectPrototype prototype) {
        var nextId = idCounter++;
        var baseLinearDrag = vehicleSystem.GetLinearDamping(holderVehicleId);
        var model = new RamEffectModel(nextId, prototype.config, holderFactionId, holderVehicleId, baseLinearDrag);
        model.Position = prototype.position;
        registry[nextId] = model;
        view.AddEffect(nextId, prototype.audioSourcePrefab);
        return nextId;
    }

    public void Remove(int ramId) {
        registry.Remove(ramId, out var model); 
        RestoreBaseLinearDrag(model);
        view.RemoveEffeect(ramId);
    }

    public void Forward(int id, Vector3 position) {
        var model = registry[id];
        model.Position = position;
    }

    private void ComputeDamage() {
        foreach (var model in registry.Values) {
            var holderIsAlie = factionRegistry.ReadState(model.HolderFactionId).alie;
            var targetRaycastLayer = FactionRegistry.GetRaycastLayerForFaction(!holderIsAlie);
            raycastService.Overlap(model.Position, model.Config.triggerRadius, targetRaycastLayer, out var idsResult);
            
            model.LostContactBuffer.Clear();
            model.LostContactBuffer.AddRange(model.InContact);
            model.ReceiveContactBuffer.Clear();
            foreach (var overlapedId in idsResult) {
                if (model.InContact.Contains(overlapedId)) {
                    model.LostContactBuffer.Remove(overlapedId);
                } else {
                    model.ReceiveContactBuffer.Add(overlapedId);
                }
            }

            model.InContact.AddRange(model.ReceiveContactBuffer);
            foreach (var lostId in model.LostContactBuffer) {
                model.InContact.Remove(lostId);
            }

            if (model.ReceiveContactBuffer.Count > 0) {
                AddTemporalLinearDrag(model, model.ReceiveContactBuffer.Count);
            }

            entityMapping.FindByRaycastIds(model.ReceiveContactBuffer, out var receiveContactComponents);
            var vehicleSpeed = vehicleSystem.ReadState(model.HolderVehicleId).velocity.magnitude;
            if (vehicleSpeed < model.Config.minImpactSpeed)
                continue;
                
            foreach (var nextComponents in receiveContactComponents) {
                if (nextComponents.motionId.HasValue) {
                    var explosionData = model.Config.explosionData;
                    explosionData.force *= Mathf.Clamp01(vehicleSpeed / model.Config.maxImpactSpeed);

                    motionSystem.AddExplosionEffect(nextComponents.motionId.Value, new Explosion {
                        epicentr = model.Position, 
                        config = explosionData
                    });
                }
                
                if (nextComponents.healthId.HasValue) {
                    healthSystem.DealDamage(nextComponents.healthId.Value, new DamageInput {
                        damageSource = model.Position,
                        damageType = DamageType.Exposion,
                        damage = model.Config.damage,
                    });
                }
            }

            if (model.ReceiveContactBuffer.Count > 0) {
                view.ShowImpact(model.Id, model.Position, model.ReceiveContactBuffer.Count, model.Config.impactSFX);
            }
        }
    }

    private void AddTemporalLinearDrag(RamEffectModel model, int contactCount) {
        var config = model.Config;
        var dragToAdd = contactCount * config.temporalLinearDragPerContact;
        model.TemporalLinearDrag = Mathf.Min(model.TemporalLinearDrag + dragToAdd, config.maxTemporalLinearDrag);
    }

    private void DecayTemporalLinearDrag() {
        foreach (var model in registry.Values) {
            var config = model.Config;
            model.TemporalLinearDrag = Mathf.MoveTowards(
                model.TemporalLinearDrag,
                0,
                config.temporalLinearDragRecoverySpeed * Time.deltaTime
            );
        }
    }

    private void ApplyTemporalLinearDrag() {
        foreach (var model in registry.Values) {
            vehicleSystem.SetLinearDamping(model.HolderVehicleId, model.BaseLinearDrag + model.TemporalLinearDrag);
        }
    }

    private void RestoreBaseLinearDrag(RamEffectModel model) {
        if (vehicleSystem.Exists(model.HolderVehicleId)) {
            vehicleSystem.SetLinearDamping(model.HolderVehicleId, model.BaseLinearDrag);
        }
    }

}
