using System.Collections.Generic;

using UnityEngine;

public class ProductionController {

    private readonly ProducerFactory producerFactory;
    private readonly PathfindingService pathfindingService;

    private ProductionModel model;

    public bool IsAnyEntityProduced => model.ProducedArmors.Count > 0 || model.ProducedInfantries.Count > 0;
    public IReadOnlyList<int> ProducedInfantries => model.ProducedInfantries;
    public IReadOnlyList<int> ProducedArmors => model.ProducedArmors;

    public ProductionController(ProducerFactory producerFactory, PathfindingService pathfindingService) {
        this.producerFactory = producerFactory;
        this.pathfindingService = pathfindingService;
    }

    public void Init(ProductionPrototype prototype) {
        model = new ProductionModel();

        foreach (var variant in prototype.producerVariants) {
            var producer = producerFactory.Create(variant);
            model.ProducerHandles.Add(new ProducerHandle(producer, variant.activationConfig));
        }
    }

    public void SetTargetFieldId(int fieldId) {
        model.TargetFlowFieldId = fieldId;
    }

    public void Update() {
        ValidateProducers();
        RegisterSpawns();
    }

    private void ValidateProducers() {
        for (int i = model.ProducerHandles.Count - 1; i >= 0; i--) {
            var producerHandle = model.ProducerHandles[i];
            if (producerHandle.IsActivated && !producerHandle.producer.IsValid()) {
                model.ProducerHandles.RemoveAt(i);
            }
            if (!producerHandle.IsActivated && (Time.time > producerHandle.activationConfig.delaySec
                || pathfindingService.GetFlowCost(model.TargetFlowFieldId, producerHandle.producer.Position) < producerHandle.activationConfig.targetTravelCostRange)) {
                producerHandle.producer.SpawnEntity();
                producerHandle.IsActivated = true;
            }
        }
    }

    private void RegisterSpawns() {
        model.ProducedArmors.Clear();
        model.ProducedInfantries.Clear();
        foreach (var producerHandle in model.ProducerHandles) {
            if (!producerHandle.IsActivated || !producerHandle.producer.TryGetSpawnResult(out var spawnResult))
                continue;

            switch (spawnResult.spawnType) {
                case SpawnVariantType.Infantry:
                    foreach (var producedInfantry in spawnResult.spawnedIds)
                        model.ProducedInfantries.Add(producedInfantry);
                    break;
                case SpawnVariantType.Armor:
                    foreach (var producedArmor in spawnResult.spawnedIds)
                        model.ProducedArmors.Add(producedArmor);
                    break;
                default: 
                    Debug.LogError($"{spawnResult.spawnType}");
                    break;
            }
        }
    }

}