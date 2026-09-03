using System.Collections.Generic;

using UnityEngine;

public class ProductionController {

    private readonly ProducerFactory producerFactory;

    private ProductionModel model;

    public bool IsAnyEntityProduced => model.ProducedArmors.Count > 0 || model.ProducedInfantries.Count > 0;
    public IReadOnlyList<int> ProducedInfantries => model.ProducedInfantries;
    public IReadOnlyList<int> ProducedArmors => model.ProducedArmors;

    public ProductionController(ProducerFactory producerFactory) {
        this.producerFactory = producerFactory;
    }

    public void Init(ProductionPrototype prototype) {
        model = new ProductionModel();

        foreach (var variant in prototype.producerVariants) {
            var producer = producerFactory.Create(variant);
            producer.SpawnEntity();
            model.Producers.Add(producer);
        }
    }

    public void Update() {
        ValidateProducers();
        RegisterSpawns();
    }

    private void ValidateProducers() {
        for (int i = model.Producers.Count - 1; i >= 0; i--) {
            var producer = model.Producers[i];
            if (!producer.IsValid()) {
                model.Producers.RemoveAt(i);
            }
        }
    }

    private void RegisterSpawns() {
        model.ProducedArmors.Clear();
        model.ProducedInfantries.Clear();
        foreach (var producer in model.Producers) {
            if (!producer.TryGetSpawnResult(out var spawnResult))
                continue;

            switch (spawnResult.spawnType) {
                case SpawnType.Infantry:
                    foreach (var producedInfantry in spawnResult.spawnedIds)
                        model.ProducedInfantries.Add(producedInfantry);
                    break;
                case SpawnType.Armor:
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