public class SpaceProducer : IProducer {

    private readonly int productionSpaceId;
    private readonly ProductionSpaceController productionSpaceController;

    public SpaceProducer(int productionSpaceId, ProductionSpaceController productionSpaceController) {
        this.productionSpaceId = productionSpaceId;
        this.productionSpaceController = productionSpaceController;
    }

    public bool IsValid() {
        return productionSpaceController.IsExist(productionSpaceId);
    }

    public bool TryGetSpawnResult(out SpawnResult spawnResult) {
        spawnResult = productionSpaceController.ReadSpawnResult(productionSpaceId);
        return spawnResult != null;
    }
}