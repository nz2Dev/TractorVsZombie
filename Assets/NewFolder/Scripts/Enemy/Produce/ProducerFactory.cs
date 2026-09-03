using System;

public class ProducerFactory {

    private readonly ProductionBuildingController productionBuildingController;
    private readonly ProductionSpaceController productionSpaceController;

    public ProducerFactory(ProductionBuildingController productionBuildingController, ProductionSpaceController productionSpaceController) {
        this.productionBuildingController = productionBuildingController;
        this.productionSpaceController = productionSpaceController;
    }

    public IProducer Create(ProducerPrototypeVariant reference) {
        return reference.type switch {
            ProducerType.ProductionSpace => new SpaceProducer(
                productionSpaceController.RegisterUniqueId(reference.producerUniqueId), 
                productionSpaceController,
                reference.productionSpacePrototype
            ),
            ProducerType.ProductionBuilding => new StructureProducer(
                productionBuildingController.RegisterUniqueId(reference.producerUniqueId), 
                productionBuildingController,
                reference.productionBuildingPrototype
            ),
            _ => throw new Exception($"{reference.type}"),
        };
    }
}