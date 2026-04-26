using System.Collections.Generic;

using UnityEngine;

public class RamEffect {

    private readonly RamView view;
    private readonly CombatSystem combatSystem;

    public RamEffect(RamView view, CombatSystem combatSystem) {
        this.view = view;
        this.combatSystem = combatSystem;
    }

    private int idCounter;
    private readonly Dictionary<int, RamModel> registry = new ();

    public void Update() {
        ComputeDamage();
    }

    public int StartNew(int combatId, RamPrototype prototype) {
        var nextId = idCounter++;
        var model = new RamModel(nextId, combatId, prototype.config);
        model.Position = prototype.position;
        registry[nextId] = model;
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
            var affectedCount = combatSystem.ApplyExplosionDamage(model.CombatId, model.Position, model.Config.radius, model.Config.damage);
            view.PlayImpact(model.Position, model.Config.radius, affectedCount, model.Config.impactSFX);
        }
    }
}