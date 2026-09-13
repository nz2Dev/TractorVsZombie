using System;

[Serializable]
public struct ProducerVariantPrototype {
    
    public ProducerType type;
    public int producerUniqueId;
    public ProductionBuildingPrototype productionBuildingPrototype;
    public SpawnerPrototype spawnerPrototype;

    public ProducerVariantPrototype(ProducerType type, int producerUniqueId, ProductionBuildingPrototype productionBuildingPrototype, SpawnerPrototype spawnerPrototype) {
        this.type = type;
        this.producerUniqueId = producerUniqueId;
        this.productionBuildingPrototype = productionBuildingPrototype;
        this.spawnerPrototype = spawnerPrototype;
    }
}
