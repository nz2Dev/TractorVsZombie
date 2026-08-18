using System.Collections.Generic;

using UnityEngine;

public class RamEffectController {

    private readonly RamEffectView view;
    private readonly CombatSystem combatSystem;

    public RamEffectController(RamEffectView view, CombatSystem combatSystem) {
        this.view = view;
        this.combatSystem = combatSystem;
    }

    private int idCounter;
    private readonly Dictionary<int, RamEffectModel> registry = new ();

    public void Update() {
        ComputeDamage();
    }

    public int StartNew(int combatId, RamEffectPrototype prototype) {
        var nextId = idCounter++;
        var model = new RamEffectModel(nextId, combatId, prototype.config);
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
            // var overlappedHitboxes = hitboxSystem.Overlap(position, radius, combatSystem.GetFoeHitboxLayer(faction));
            // var overlappedInfantries = infantryController.GetEntitiesByHitboxes(overlappedHitboxes);
            // var groundedEntities = overlappedInfantries.Filter(entity => entity.movement.IsGrounded);
            // movementSystem.Explode(groundedEntities.MovementIds, explosion);
            // combatSystem.DealDamage(groundedEntities.CombatIds, explosionDamage);

            var affectedCount = combatSystem.ApplyExplosionDamage(model.CombatId, model.Position, 
                model.Config.triggerRadius, model.Config.damage, model.Config.explosionData);
            view.ShowImpact(model.Id, model.Position, affectedCount, model.Config.impactSFX);
        }
    }
}