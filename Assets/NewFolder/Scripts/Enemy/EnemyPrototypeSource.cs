using System.Diagnostics;
using System.Linq;

using UnityEngine;

public class EnemyPrototypeSource : MonoBehaviour {
    [Inline] [SerializeField] private EnemyConfig configSource;
    [SerializeField] private CommanderSource[] commanderSources;
    [SerializeField] private ProductionBuildingSource[] productionBuildingSources;
    [SerializeField] private ProductionSpaceSource[] productionSpaceSources;

    public void FindInScene() {
        var sources = GameObject.FindObjectsByType<ProductionSpaceSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var productionBuildingSources = GameObject.FindObjectsByType<ProductionBuildingSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var commanderSources = GameObject.FindObjectsByType<CommanderSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);

    }

    public EnemyPrototype Get() {
        return new EnemyPrototype {
            enemyConfig = configSource,
            commanderPrototypes = commanderSources == null ? new CommanderPrototype[0] : commanderSources.Select(source => source.GetPrototype()).ToArray(),
            productionBuildingPrototypes = productionBuildingSources == null 
                ? new ProductionBuildingPrototype[0] : productionBuildingSources.Select(source => source.GetPrototype()).ToArray(),
            productionSpacePrototypes = productionSpaceSources == null
                ? new ProductionSpacePrototype[0] : productionSpaceSources.Select(source => source.GetPrototype()).ToArray()
        };
    }
}