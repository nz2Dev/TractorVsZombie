using System;

[Serializable]
public struct ProductionSource {
    
    public ProductionBuildingSource[] productionBuildingSources;
    public SpawnerSource[] spawnerSources;

    public readonly ProductionPrototype Build() {
        return new ProductionPrototype (
            producerVariants: BuildProducerVariants()
        );
    }

    private readonly ProducerPrototypeVariant[] BuildProducerVariants() {
        var referencesLength = productionBuildingSources.Length + spawnerSources.Length;
        var variant = new ProducerPrototypeVariant[referencesLength];

        var index = 0;
        foreach (var buildingSource in productionBuildingSources) {
            variant[index++] = new ProducerPrototypeVariant (
                producerUniqueId: buildingSource.GetUniqueId(),
                type: ProducerType.ProductionBuilding,
                productionBuildingPrototype: buildingSource.GetPrototype(),
                spawnerPrototype: default
            );
        }

        foreach (var spawnerSource in spawnerSources) {
            variant[index++] = new ProducerPrototypeVariant (
                producerUniqueId: -1,
                type: ProducerType.Spawner,
                spawnerPrototype: spawnerSource.Get(),
                productionBuildingPrototype: default
            );
        }

        return variant;
    }
}