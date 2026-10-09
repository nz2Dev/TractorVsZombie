using System;

[Serializable]
public struct ProducerVariantSource {
    
    public ProducerType producerType;
    public ProductionBuildingSource productionBuildingSource;
    public SpawnerPrototypeSource spawnerPrototypeSource;
    public ProducerActivationConfig activationConfig;

    public readonly ProducerVariantPrototype Get() {
        return new ProducerVariantPrototype(
            producerUniqueId: ObtainUniqueId(),
            type: producerType,
            productionBuildingPrototype: productionBuildingSource == null ? default : productionBuildingSource.GetPrototype(),
            spawnerPrototype: spawnerPrototypeSource == null ? default : spawnerPrototypeSource.Get(),
            activationConfig: activationConfig
        );
    }

    private readonly int ObtainUniqueId() {
        if (producerType == ProducerType.ProductionBuilding) {
            return productionBuildingSource.GetUniqueId();
        } else {
            return -1;
        }
    }
}