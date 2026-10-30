using System.Collections.Generic;
using UnityEngine;

public class LoaderController {
    
    private readonly PlatformController platformController;
    private readonly AssemblingController assemblingController;

    public LoaderController(PlatformController platformController, AssemblingController assemblingController) {
        this.platformController = platformController;
        this.assemblingController = assemblingController;
    }

    public void Place(LoadoutPrototype loadout) {
        var platformIds = assemblingController.ControlledPlatformIds;
        var platformStates = new List<PlatformState>(platformIds.Count);
        foreach (var platformId in platformIds) {
            if (platformController.Exist(platformId)) {
                platformStates.Add(platformController.ReadPlatformState(platformId));
            }
        }

        if (platformStates.Count == 0) {
            Debug.LogWarning("Cannot place loadout because no active platforms are available.");
            return;
        }

        var freeSlotIndex = platformStates.FindIndex(state => !state.hasLoadout);
        var target = platformStates[freeSlotIndex >= 0 ? freeSlotIndex : 0];
        platformController.SetLoadout(target.platformId, loadout);
    }
    
}
