using System;

[Serializable]
public struct ProducerPrototypeVariant {
    
    public ProducerType type;
    public int producerUniqueId;
    public ProductionBuildingPrototype productionBuildingPrototype;
    public SpawnerPrototype spawnerPrototype;

    public ProducerPrototypeVariant(int producerUniqueId, ProducerType type, ProductionBuildingPrototype productionBuildingPrototype, SpawnerPrototype spawnerPrototype) {
        this.producerUniqueId = producerUniqueId;
        this.type = type;
        this.productionBuildingPrototype = productionBuildingPrototype;
        this.spawnerPrototype = spawnerPrototype;
    }
}
