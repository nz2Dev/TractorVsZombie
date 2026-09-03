using System.Diagnostics;
using System.Linq;

using UnityEngine;

public class EnemyPrototypeSource : MonoBehaviour {
    [Inline] [SerializeField] private EnemyConfig configSource;
    [Inline, SerializeField] private InfantryAIConfig infantryAIConfig;
    [SerializeField] private ProductionSource productionSource;

    public void FindInScene() {
        var sources = GameObject.FindObjectsByType<ProductionSpaceSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var productionBuildingSources = GameObject.FindObjectsByType<ProductionBuildingSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    public EnemyPrototype Get() {
        return new EnemyPrototype (
            enemyConfig: configSource,
            infantryAIConfig: infantryAIConfig,
            productionPrototype: productionSource.Build()
        );
    }

}