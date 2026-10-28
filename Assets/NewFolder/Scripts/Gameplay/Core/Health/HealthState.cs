public struct HealthState {
    public int health;
    public int maxHealth;
    public DamageResult? damageResult;
    public bool isDead;

    public HealthState(int health, int maxHealth, DamageResult? damageResult, bool isDead) {
        this.health = health;
        this.maxHealth = maxHealth;
        this.damageResult = damageResult;
        this.isDead = isDead;
    }
}
