public struct EnemyPrototype {
    public EnemyConfig enemyConfig;
    public InfantryAIConfig infantryAIConfig;
    public ProductionPrototype productionPrototype;

    public EnemyPrototype(EnemyConfig enemyConfig, InfantryAIConfig infantryAIConfig, ProductionPrototype productionPrototype) {
        this.enemyConfig = enemyConfig;
        this.infantryAIConfig = infantryAIConfig;
        this.productionPrototype = productionPrototype;
    }
}