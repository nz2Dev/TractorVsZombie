using System.Diagnostics;
using System.Linq;

using UnityEngine;

public class EnemyPrototypeSource : MonoBehaviour {
    [Inline] [SerializeField] private EnemyConfig configSource;
    [Inline, SerializeField] private InfantryAIConfig infantryAIConfig;
    [SerializeField] private ProductionBuildingSource[] productionBuildingSources;
    [SerializeField] private ProductionSpaceSource[] productionSpaceSources;

    public void FindInScene() {
        var sources = GameObject.FindObjectsByType<ProductionSpaceSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var productionBuildingSources = GameObject.FindObjectsByType<ProductionBuildingSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    public EnemyPrototype Get() {
        return new EnemyPrototype {
            enemyConfig = configSource,
            infantryAIConfig = infantryAIConfig,
            producerVariants = BuildProducerReferences()
        };
    }

    private ProducerPrototypeVariant[] BuildProducerReferences() {
        var referencesLength = productionBuildingSources.Length + productionSpaceSources.Length;
        var references = new ProducerPrototypeVariant[referencesLength];

        var index = 0;
        foreach (var buildingSource in productionBuildingSources) {
            references[index++] = new ProducerPrototypeVariant (
                producerUniqueId: buildingSource.GetUniqueId(),
                type: ProducerType.ProductionBuilding,
                productionBuildingPrototype: buildingSource.GetPrototype(),
                productionSpacePrototype: default
            );
        }

        foreach (var spaceSource in productionSpaceSources) {
            references[index++] = new ProducerPrototypeVariant (
                producerUniqueId: spaceSource.GetUniqueId(),
                type: ProducerType.ProductionSpace,
                productionSpacePrototype: spaceSource.GetPrototype(),
                productionBuildingPrototype: default
            );
        }

        return references;
    }
}