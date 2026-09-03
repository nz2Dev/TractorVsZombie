using System;

[Serializable]
public enum ProducerType {
    ProductionSpace,
    ProductionBuilding
}

[Serializable]
public struct ProducerPrototypeVariant {
    
    public ProducerType type;
    public int producerUniqueId;
    public ProductionBuildingPrototype productionBuildingPrototype;
    public ProductionSpacePrototype productionSpacePrototype;

    public ProducerPrototypeVariant(int producerUniqueId, ProducerType type, ProductionBuildingPrototype productionBuildingPrototype, ProductionSpacePrototype productionSpacePrototype) {
        this.producerUniqueId = producerUniqueId;
        this.type = type;
        this.productionBuildingPrototype = productionBuildingPrototype;
        this.productionSpacePrototype = productionSpacePrototype;
    }
}

public interface IProducer {

    bool IsValid();
    void SpawnEntity();
    bool TryGetSpawnResult(out SpawnResult spawnResult);

}