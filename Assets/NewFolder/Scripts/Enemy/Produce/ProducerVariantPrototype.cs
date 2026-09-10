using System;

[Serializable]
public struct ProducerVariantPrototype {
    
    public ProducerType type;
    public int producerUniqueId;
    public ProductionBuildingPrototype productionBuildingPrototype;
    public SpawnerPrototype spawnerPrototype;
    public SpawnSetup spawnSetup;

    public ProducerVariantPrototype(int producerUniqueId, ProducerType type, ProductionBuildingPrototype productionBuildingPrototype, SpawnerPrototype spawnerPrototype, SpawnSetup spawnSetup) {
        this.producerUniqueId = producerUniqueId;
        this.type = type;
        this.productionBuildingPrototype = productionBuildingPrototype;
        this.spawnerPrototype = spawnerPrototype;
        this.spawnSetup = spawnSetup;
    }
}
