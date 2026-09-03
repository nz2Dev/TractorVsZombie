using System;

[Serializable]
public struct ProductionSource {
    
    public ProductionBuildingSource[] productionBuildingSources;
    public ProductionSpaceSource[] productionSpaceSources;

    public readonly ProductionPrototype Build() {
        return new ProductionPrototype (
            producerVariants: BuildProducerVariants()
        );
    }

    private readonly ProducerPrototypeVariant[] BuildProducerVariants() {
        var referencesLength = productionBuildingSources.Length + productionSpaceSources.Length;
        var variant = new ProducerPrototypeVariant[referencesLength];

        var index = 0;
        foreach (var buildingSource in productionBuildingSources) {
            variant[index++] = new ProducerPrototypeVariant (
                producerUniqueId: buildingSource.GetUniqueId(),
                type: ProducerType.ProductionBuilding,
                productionBuildingPrototype: buildingSource.GetPrototype(),
                productionSpacePrototype: default
            );
        }

        foreach (var spaceSource in productionSpaceSources) {
            variant[index++] = new ProducerPrototypeVariant (
                producerUniqueId: spaceSource.GetUniqueId(),
                type: ProducerType.ProductionSpace,
                productionSpacePrototype: spaceSource.GetPrototype(),
                productionBuildingPrototype: default
            );
        }

        return variant;
    }
}