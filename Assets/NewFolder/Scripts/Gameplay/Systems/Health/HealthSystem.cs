using System.Collections.Generic;

public class HealthSystem {

    private int idCounter;
    private readonly Dictionary<HealthId, HealthModel> models = new();

    public HealthSystem() {
    }

    public HealthId Add(HealthPrototype prototype) {
        var nextId = new HealthId(++idCounter);
        var model = new HealthModel(nextId, prototype.config);
        model.Health = model.Config.maxHelath;
        models[nextId] = model;
        return nextId;
    }

    public void Remove(HealthId healthId) {
        models.Remove(healthId);
    }

    public void DealDamage(HealthId target, DamageInput damageInput) {
        var targetModel = models[target];
        targetModel.DamageInput = damageInput; // IMPORTANT: doesn't accumulate
    }

    public HealthState ReadState(HealthId id) {
        var model = models[id];
        return new HealthState (
            health: model.Health,
            maxHealth: model.Config.maxHelath,
            damageResult: model.DamageResult,
            surface: model.Config.surface,
            isDead: model.Dead
        );
    }

    public void Update() {
        foreach (var model in models.Values) {
            model.DamageResult = null;
            if (model.Dead) 
                continue;

            if (model.DamageInput.HasValue) {
                var input = model.DamageInput.Value;
                model.DamageInput = null;
                model.Health -= input.damage;
                if (model.Health <= 0) {
                    model.Dead = true;
                }
                
                model.DamageResult = new DamageResult {
                    damageType = input.damageType,
                    damageWasFatal = model.Health <= 0,
                    damageSource = input.damageSource,
                    damage = input.damage,
                };
            }
        }
    }

}
