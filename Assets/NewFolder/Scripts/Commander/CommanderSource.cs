using System.Linq;

using UnityEngine;

public class CommanderSource : MonoBehaviour {
    
    [Inline] [SerializeField] private CommanderConfig commanderConfig;
    [SerializeField] private ProductionBuildingSource[] productionBuildingSource;
    [SerializeField] private ProductionSpaceSource[] productionSpaceSources;

    public CommanderPrototype GetPrototype() {
        var handlesLength = productionBuildingSource.Length + productionSpaceSources.Length;
        var handles = new ProducerReference[handlesLength];

        var index = 0;
        foreach (var buildingSource in productionBuildingSource) {
            handles[index++] = new ProducerReference {
                producerUniqueId = buildingSource.GetUniqueId(),
                type = ProducerType.ProductionBuilding
            };
        }

        foreach (var spaceSource in productionSpaceSources) {
            handles[index++] = new ProducerReference {
                producerUniqueId = spaceSource.GetUniqueId(),
                type = ProducerType.ProductionSpace
            };
        }

        return new CommanderPrototype {
            commanderConfig = commanderConfig,
            position = transform.position,
            producerHandles = handles
        };
    }

}