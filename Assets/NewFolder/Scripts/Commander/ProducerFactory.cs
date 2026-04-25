using System;

public class ProducerFactory {

    private readonly ProductionBuildingController productionBuildingController;
    private readonly ProductionSpaceController productionSpaceController;

    public ProducerFactory(ProductionBuildingController productionBuildingController, ProductionSpaceController productionSpaceController) {
        this.productionBuildingController = productionBuildingController;
        this.productionSpaceController = productionSpaceController;
    }

    public IProducer Create(ProducerHandle handle) {
        return handle.type switch {
            ProducerType.Space => new SpaceProducer(handle.producerId, productionSpaceController),
            ProducerType.Structure => new StructureProducer(handle.producerId, productionBuildingController),
            _ => throw new Exception($"{handle.type}"),
        };
    }

}