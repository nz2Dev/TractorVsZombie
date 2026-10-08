namespace Combat {
    public struct CombatState {
        public bool alie;
        public int health;
        public int maxHealth;
        public DamageResult? damageResult;
        public ContactSurface surface;
        public bool isDead;

        public CombatState(bool alie, int health, int maxHealth, DamageResult? damageResult, ContactSurface surface, bool isDead) {
            this.alie = alie;
            this.health = health;
            this.maxHealth = maxHealth;
            this.damageResult = damageResult;
            this.surface = surface;
            this.isDead = isDead;
        }
    }
}
