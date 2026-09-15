namespace Combat {
    public struct CombatState {
        public bool alie;
        public int health;
        public int maxHealth;
        public DamageResult? damageResult;
        public ContactSurface surface;

        public CombatState(bool alie, int health, int maxHealth, DamageResult? damageResult, ContactSurface surface) {
            this.alie = alie;
            this.health = health;
            this.maxHealth = maxHealth;
            this.damageResult = damageResult;
            this.surface = surface;
        }
    }
}
