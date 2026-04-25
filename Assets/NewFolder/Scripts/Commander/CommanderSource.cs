using System.Linq;

using UnityEngine;

public class CommanderSource : MonoBehaviour {
    
    [SerializeField] private CommanderConfig commanderConfig;
    [SerializeField] private ProductionBuildingSource[] productionBuildingSource; // has no effect for now
    [SerializeField] private ProductionSpaceSource[] productionSpaceSources; // has no effect for now

    public CommanderPrototype GetPrototype() {
        return new CommanderPrototype {
            commanderConfig = commanderConfig,
            position = transform.position,
            // todo implement external ids. productionBuildingId = productionBuildingPlace.
            producerHandles = new ProducerHandle[] {
                new() {
                    producerId = 1, // will work, assuming there will be only one such building in runtime created
                    type = ProducerType.Structure
                },
                new() {
                    producerId = 1, // same here
                    type = ProducerType.Space
                }
            }, 
        };
    }

}