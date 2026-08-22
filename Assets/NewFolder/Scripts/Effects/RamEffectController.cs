using System.Collections.Generic;

using Combat;

using UnityEngine;

public class RamEffectController {

    private readonly RamEffectView view;
    private readonly CombatSystem combatSystem;
    private readonly RaycastService raycastService;
    private readonly InfantryController infantryController;

    public RamEffectController(RamEffectView view, CombatSystem combatSystem, InfantryController infantryController, RaycastService raycastService) {
        this.view = view;
        this.combatSystem = combatSystem;
        this.infantryController = infantryController;
        this.raycastService = raycastService;
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
                var exploded = infantryController.Explode(nextInfantryId, 
                    model.Position, model.Config.explosionData);
                
                if (exploded) {
                    affectedCount++;
                }

                var nextInfantryState = infantryController.GetInfantryState(nextInfantryId);
                combatSystem.DealDamage(nextInfantryState.combatId, new DamageInput {
                    damageSource = model.Position,
                    damageType = DamageType.Exposion,
                    damage = model.Config.damage,
                });
            }

            // TODO: search for other entities..

            if (affectedCount > 0) {
                view.ShowImpact(model.Id, model.Position, affectedCount, model.Config.impactSFX);
            }
        }
    }
}
