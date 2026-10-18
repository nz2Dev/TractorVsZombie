public struct HealthState {
    public int health;
    public int maxHealth;
    public DamageResult? damageResult;
    public ContactSurface surface;
    public bool isDead;

    public HealthState(int health, int maxHealth, DamageResult? damageResult, ContactSurface surface, bool isDead) {
        this.health = health;
        this.maxHealth = maxHealth;
        this.damageResult = damageResult;
        this.surface = surface;
        this.isDead = isDead;
    }
}
