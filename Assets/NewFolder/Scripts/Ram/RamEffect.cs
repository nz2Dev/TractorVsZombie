using System.Collections.Generic;

using UnityEngine;

public class RamEffect {

    private readonly CombatService combatService;
    private readonly SoundManager soundManager;

    public RamEffect(CombatService combatService, SoundManager soundManager) {
        this.combatService = combatService;
        this.soundManager = soundManager;
    }

    private int idCounter;
    private Dictionary<int, RamModel> registry = new ();

    public void Update() {
        ComputeDamage();
    }

    public int StartNew(Vector3 position, int combatId, RamConfig config) {
        var nextId = idCounter++;
        var model = new RamModel(nextId, combatId, position, config);
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
            var affectedCount = combatService.ApplyExplosionDamage(model.CombatId, model.Position, model.Radius, damage: 0);
            for (int i = 0; i < affectedCount; i++) {
                var position = model.Position + Random.onUnitSphere * model.Radius;
                soundManager.PlayEffectDelayed(position, i * 0.05f, model.ImpactSFX);
            }    
        }
    }
}