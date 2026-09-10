using System;

public class ProducerFactory {

    private readonly ProductionBuildingController productionBuildingController;
    private readonly SpawnerController spawnerController;

    public ProducerFactory(ProductionBuildingController productionBuildingController, SpawnerController spawnerController) {
        this.productionBuildingController = productionBuildingController;
        this.spawnerController = spawnerController;
    }

    public IProducer Create(ProducerVariantPrototype reference) {
        return reference.type switch {
            ProducerType.Spawner => new SpawnerProducer(
                reference.spawnerPrototype,
                spawnerController,
                reference.spawnSetup
            ),
            ProducerType.ProductionBuilding => new StructureProducer(
                productionBuildingController.RegisterUniqueId(reference.producerUniqueId), 
                productionBuildingController,
                reference.productionBuildingPrototype,
                spawnerController,
                reference.spawnSetup
            ),
            _ => throw new Exception($"{reference.type}"),
        };
    }
}