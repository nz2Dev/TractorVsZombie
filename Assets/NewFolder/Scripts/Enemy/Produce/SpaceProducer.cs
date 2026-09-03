public class SpaceProducer : IProducer {

    private readonly int productionSpaceId;
    private readonly ProductionSpacePrototype productionSpacePrototype;
    private readonly ProductionSpaceController productionSpaceController;

    public SpaceProducer(int productionSpaceId, ProductionSpaceController productionSpaceController, ProductionSpacePrototype productionSpacePrototype) {
        this.productionSpaceId = productionSpaceId;
        this.productionSpaceController = productionSpaceController;
        this.productionSpacePrototype = productionSpacePrototype;
    }

    public bool IsValid() {
        return productionSpaceController.IsExist(productionSpaceId);
    }

    public void SpawnEntity() {
        productionSpaceController.Create(productionSpacePrototype);
    }

    public bool TryGetSpawnResult(out SpawnResult spawnResult) {
        spawnResult = productionSpaceController.ReadSpawnResult(productionSpaceId);
        return spawnResult != null;
    }
}