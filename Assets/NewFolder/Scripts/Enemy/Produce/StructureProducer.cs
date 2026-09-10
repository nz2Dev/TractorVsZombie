public class StructureProducer : IProducer {

    private readonly int productionBuildingId;
    private readonly ProductionBuildingController productionBuildingController;
    private readonly ProductionBuildingPrototype productionBuildingPrototype;
    private readonly SpawnerController spawnerController;
    private readonly SpawnSetup initSetup;

    public StructureProducer(int productionBuildingId, ProductionBuildingController productionBuildingController, ProductionBuildingPrototype productionBuildingPrototype, SpawnerController spawnerController, SpawnSetup initSetup) {
        this.productionBuildingId = productionBuildingId;
        this.productionBuildingController = productionBuildingController;
        this.productionBuildingPrototype = productionBuildingPrototype;
        this.spawnerController = spawnerController;
        this.initSetup = initSetup;
    }

    public bool IsValid() {
        return productionBuildingController.IsExist(productionBuildingId);
    }

    public void SpawnEntity() {
        productionBuildingController.Create(productionBuildingPrototype);
        var state = productionBuildingController.ReadState(productionBuildingId);
        spawnerController.Configure(state.spawnerId, initSetup);
    }

    public bool TryGetSpawnResult(out SpawnResult spawnResult) {
        spawnResult = productionBuildingController.ReadState(productionBuildingId).lastResult;
        return spawnResult != null;
    }
}