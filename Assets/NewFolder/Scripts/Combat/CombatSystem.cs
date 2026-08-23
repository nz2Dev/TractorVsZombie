using System.Collections.Generic;

namespace Combat  {
    public class CombatSystem {

        public static ReservedLayerCode GetRaycastLayerForFaction(bool alie) => 
            alie ? ReservedLayerCode.First : ReservedLayerCode.Second;
        public static ProximityService.Layer GetProximityLayerForFaction(bool alie) => 
            alie ? ProximityService.Layer.CombatReservedA : ProximityService.Layer.CombatReservedB;

        private int idCounter;
        private readonly Dictionary<CombatId, CombatModel> models = new();

        public CombatSystem() {
        }

        public CombatId Add(CombatPrototype prototype) {
            var nextId = new CombatId(++idCounter);
            var model = new CombatModel(nextId, prototype.config, prototype.alie);
            model.Health = model.Config.maxHelath;
            models[nextId] = model;
            return nextId;
        }

        public void Remove(CombatId combatId) {
            models.Remove(combatId);
        }

        public void DealDamage(CombatId target, DamageInput damageInput) {
            var targetModel = models[target];
            targetModel.DamageInput = damageInput; // IMPORTANT: doesn't accumulate
        }

        public CombatState ReadState(CombatId id) {
            var model = models[id];
            return new CombatState {
                alie = model.Alie,
                health = model.Health,
                damageResult = model.DamageResult,
                surface = model.Config.surface,
            };
        }

        public void Update() {
            foreach (var model in models.Values) {
                model.DamageResult = null;
                if (model.DamageInput.HasValue) {
                    var input = model.DamageInput.Value;
                    model.DamageInput = null;
                    model.Health -= input.damage;
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
}
