using System.Collections.Generic;

using Combat;

using Interactions;

using UnityEngine;

public class RamEffectController {

    private readonly RamEffectView view;
    private readonly CombatSystem combatSystem;
    private readonly RaycastService raycastService;
    private readonly InteractionRegistry interactionRegistry;
    private readonly InfantryController infantryController;

    public RamEffectController(RamEffectView view, CombatSystem combatSystem, InfantryController infantryController, RaycastService raycastService, InteractionRegistry interactionRegistry) {
        this.view = view;
        this.combatSystem = combatSystem;
        this.infantryController = infantryController;
        this.raycastService = raycastService;
        this.interactionRegistry = interactionRegistry;
    }

    private int idCounter;
    private readonly Dictionary<int, RamEffectModel> registry = new ();

    public void Update() {
        ComputeDamage();
    }

    public int StartNew(CombatId holderCombatId, bool holderIsAlie, RamEffectPrototype prototype) {
        var nextId = idCounter++;
        var model = new RamEffectModel(nextId, prototype.config, holderCombatId, holderIsAlie);
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
            infantryController.FindByRaycastIds(idsResult, out var infantryIdsResult);
            
            var affectedCount = 0;
            foreach (var nextInfantryId in infantryIdsResult) {
                var nextInfantry = infantryController.GetInfantryState(nextInfantryId);
                var interactionState = interactionRegistry.Read(nextInfantry.interactionId);
                
                if (nextInfantry.isGrounded && interactionState.activeEffect != EffectType.Explosion) {
                    affectedCount++;
                    // todo: keep track of raycasted objects "in contact", to prevent continuous explosion effects triggering
                    // instead of relying on internal state IsGrounded
                    interactionRegistry.AddExplosionEffect(nextInfantry.interactionId, new Explosion {
                        epicentr = model.Position, 
                        config = model.Config.explosionData
                    });
                    
                    combatSystem.DealDamage(nextInfantry.combatId, new DamageInput {
                        damageSource = model.Position,
                        damageType = DamageType.Exposion,
                        damage = model.Config.damage,
                    });
                }
            }

            // TODO: search for other entities..

            if (affectedCount > 0) {
                view.ShowImpact(model.Id, model.Position, /*affectedCont*/1, model.Config.impactSFX);
            }
        }
    }
}
