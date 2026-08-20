namespace Combat {
    public class CombatModel {

        public CombatModel(CombatId id, CombatConfig config, bool alie) {
            Id = id;
            Config = config;
            Alie = alie;
        }

        public CombatId Id { get; }
        public CombatConfig Config { get; }
        public bool Alie { get; }

        public int Health { get; set; }
        public DamageInput? DamageInput { get; set; }
        public DamageResult? DamageResult { get; set; }

    }
}
