public class StructureProducer : IProducer {

    private readonly int productionBuildingId;
    private readonly ProductionBuildingController productionBuildingController;

    public StructureProducer(int productionBuildingId, ProductionBuildingController productionBuildingController) {
        this.productionBuildingId = productionBuildingId;
        this.productionBuildingController = productionBuildingController;
    }

    public bool IsValid() {
        return productionBuildingController.IsExist(productionBuildingId);
    }

    public bool TryGetSpawnResult(out SpawnResult spawnResult) {
        spawnResult = productionBuildingController.ReadState(productionBuildingId).lastResult;
        return spawnResult != null;
    }
}