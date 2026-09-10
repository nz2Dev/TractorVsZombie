using System;

[Serializable]
public struct ProducerVariantSource {
    
    public ProducerType producerType;
    public SpawnerSource spawnerSource;
    public ProductionBuildingSource productionBuildingSource;
    public SpawnConfig spawnConfig;
    [Inline] public SpawnSpotSource spawnSpotSource;
    public SpawnVariantSource spawnVariant;

    public readonly ProducerVariantPrototype Get() {
        return new ProducerVariantPrototype(
            producerUniqueId: ObtainUniqueId(),
            type: producerType,
            productionBuildingPrototype: productionBuildingSource == null ? default : productionBuildingSource.GetPrototype(),
            spawnerPrototype: spawnerSource.Get(),
            spawnSetup: new SpawnSetup(
                config: spawnConfig,
                spot: spawnSpotSource == null ? default : spawnSpotSource.Get(),
                variant: spawnVariant.Get()
            )
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