using UnityEngine;

public class CommanderSource : MonoBehaviour {
    
    [SerializeField] private CommanderConfig commanderConfig;
    [SerializeField] private ProductionBuildingSource productionBuildingSource; // has no effect for now

    public CommanderPrototype GetPrototype() {
        return new CommanderPrototype {
            commanderConfig = commanderConfig,
            position = transform.position,
            // todo implement external ids. productionBuildingId = productionBuildingPlace.
            productionBuildingId = 1, // will work, assuming there will be only one such building in runtime created
        };
    }

}