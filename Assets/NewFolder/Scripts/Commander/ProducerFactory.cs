using System;

public class ProducerFactory {

    private readonly ProductionBuildingController productionBuildingController;
    private readonly ProductionSpaceController productionSpaceController;

    public ProducerFactory(ProductionBuildingController productionBuildingController, ProductionSpaceController productionSpaceController) {
        this.productionBuildingController = productionBuildingController;
        this.productionSpaceController = productionSpaceController;
    }

    public IProducer Create(ProducerReference reference) {
        return reference.type switch {
            ProducerType.ProductionSpace => new SpaceProducer(
                productionSpaceController.RegisterUniqueId(reference.producerUniqueId), 
                productionSpaceController
            ),
            ProducerType.ProductionBuilding => new StructureProducer(
                productionBuildingController.RegisterUniqueId(reference.producerUniqueId), 
                productionBuildingController
            ),
            _ => throw new Exception($"{reference.type}"),
        };
    }
}