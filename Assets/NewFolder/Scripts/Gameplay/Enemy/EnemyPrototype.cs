public struct EnemyPrototype {
    public EnemyConfig enemyConfig;
    public InfantryAIConfig infantryAIConfig;
    public ProductionPrototype productionPrototype;
    public GoalsPrototype goalsPrototype;

    public EnemyPrototype(EnemyConfig enemyConfig, InfantryAIConfig infantryAIConfig, ProductionPrototype productionPrototype, GoalsPrototype goalsPrototype) {
        this.enemyConfig = enemyConfig;
        this.infantryAIConfig = infantryAIConfig;
        this.productionPrototype = productionPrototype;
        this.goalsPrototype = goalsPrototype;
    }
}