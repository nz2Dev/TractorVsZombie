public class StructureProducer : IProducer {

    private readonly int productionBuildingId;
    private readonly ProductionBuildingController productionBuildingController;
    private readonly ProductionBuildingPrototype productionBuildingPrototype;

    public StructureProducer(int productionBuildingId, ProductionBuildingController productionBuildingController, ProductionBuildingPrototype productionBuildingPrototype) {
        this.productionBuildingId = productionBuildingId;
        this.productionBuildingController = productionBuildingController;
        this.productionBuildingPrototype = productionBuildingPrototype;
    }

    public bool IsValid() {
        return productionBuildingController.IsExist(productionBuildingId);
    }

    public void SpawnEntity() {
        productionBuildingController.Create(productionBuildingPrototype);
    }

    public bool TryGetSpawnResult(out SpawnResult spawnResult) {
        spawnResult = productionBuildingController.ReadState(productionBuildingId).lastResult;
        return spawnResult != null;
    }
}