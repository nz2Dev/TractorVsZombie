using System.Collections.Generic;
using UnityEngine;

public class LoaderController {
    
    private readonly PlatformController platformController;
    private readonly AssemblingController assemblingController;

    public LoaderController(PlatformController platformController, AssemblingController assemblingController) {
        this.platformController = platformController;
        this.assemblingController = assemblingController;
    }

    public bool TryPlace(LoadoutPrototype loadout) {
        var platformIds = assemblingController.ControlledPlatformIds;
        var platformStates = new List<PlatformState>(platformIds.Count);
        foreach (var platformId in platformIds) {
            if (platformController.Exist(platformId)) {
                platformStates.Add(platformController.ReadPlatformState(platformId));
            }
        }

        if (platformStates.Count == 0) {
            Debug.LogWarning("Cannot place loadout because no active platforms are available.");
            return false;
        }

        var freeSlotIndex = platformStates.FindIndex(state => !state.hasLoadout);
        if (freeSlotIndex < 0) {
            return false;
        }

        var target = platformStates[freeSlotIndex];
        platformController.SetLoadout(target.platformId, loadout);
        return true;
    }
    
}
