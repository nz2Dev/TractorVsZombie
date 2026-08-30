using System.Collections.Generic;

using Combat;

using Interactions;

using UnityEngine;

public class RamEffectController {

    private readonly RamEffectView view;
    private readonly CombatSystem combatSystem;
    private readonly RaycastService raycastService;
    private readonly VehicleService vehicleService;
    private readonly InteractionRegistry interactionRegistry;
    private readonly EntityMapping entityMapping;

    public RamEffectController(RamEffectView view, CombatSystem combatSystem, RaycastService raycastService, InteractionRegistry interactionRegistry, EntityMapping entityMapping, VehicleService vehicleService) {
        this.view = view;
        this.combatSystem = combatSystem;
        this.raycastService = raycastService;
        this.interactionRegistry = interactionRegistry;
        this.entityMapping = entityMapping;
        this.vehicleService = vehicleService;
    }

    private int idCounter;
    private readonly Dictionary<int, RamEffectModel> registry = new ();

    public void Update() {
        ComputeDamage();
    }

    public int StartNew(CombatId holderCombatId, int holderVehicleId, bool holderIsAlie, RamEffectPrototype prototype) {
        var nextId = idCounter++;
        var model = new RamEffectModel(nextId, prototype.config, holderCombatId, holderVehicleId, holderIsAlie);
        model.Position = prototype.position;
        registry[nextId] = model;
        view.AddEffect(nextId, prototype.audioSourcePrefab);
        return nextId;
    }

    public void Forward(int id, Vector3 position) {
        var model = registry[id];
        model.Position = position;
    }

    public void Stop(int id) {
        registry.Remove(id);
    }

    private void ComputeDamage() {
        foreach (var model in registry.Values) {
            var targetRaycastLayer = CombatSystem.GetRaycastLayerForFaction(!model.HolderIsAlie);
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

            entityMapping.FindByRaycastIds(model.ReceiveContactBuffer, out var receiveContactComponents);
            foreach (var nextComponents in receiveContactComponents) {
                if (nextComponents.interactionId.HasValue) {
                    interactionRegistry.AddExplosionEffect(nextComponents.interactionId.Value, new Explosion {
                        epicentr = model.Position, 
                        config = model.Config.explosionData
                    });
                }
                
                if (nextComponents.combatId.HasValue) {
                    combatSystem.DealDamage(nextComponents.combatId.Value, new DamageInput {
                        damageSource = model.Position,
                        damageType = DamageType.Exposion,
                        damage = model.Config.damage,
                    });
                }
            }

            var dragAmount = Mathf.Min(model.ReceiveContactBuffer.Count, model.Config.maxDragInteraction) / (float) model.Config.maxDragInteraction;
            vehicleService.ApplyDragForce(model.HolderVehicleId, dragAmount * model.Config.maxDragForce, model.Config.dragForceMode);

            if (model.ReceiveContactBuffer.Count > 0) {
                view.ShowImpact(model.Id, model.Position, model.ReceiveContactBuffer.Count, model.Config.impactSFX);
            }
        }
    }
}
