using System;

[Serializable]
public struct ProducerVariantPrototype {
    
    public ProducerType type;
    public ProducerActivationConfig activationConfig;
    public int producerUniqueId;
    public ProductionBuildingPrototype productionBuildingPrototype;
    public SpawnerPrototype spawnerPrototype;

    public ProducerVariantPrototype(ProducerType type, int producerUniqueId, ProductionBuildingPrototype productionBuildingPrototype, SpawnerPrototype spawnerPrototype, ProducerActivationConfig activationConfig) {
        this.type = type;
        this.producerUniqueId = producerUniqueId;
        this.productionBuildingPrototype = productionBuildingPrototype;
        this.spawnerPrototype = spawnerPrototype;
        this.activationConfig = activationConfig;
    }
}
