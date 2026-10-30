using System.Collections.Generic;

public class LoaderController {
    
    private readonly PlatformController platformController;
    private LoaderModel model = new();

    public LoaderController(PlatformController platformController) {
        this.platformController = platformController;
    }

    public void Init() {
        model = new LoaderModel();
    }

    public void Place(LoadoutPrototype loadout) {
        if (model.PlatformIds.Count == 0) {
            model.PendingLoadouts.Enqueue(loadout);
            return;
        }

        var platformStates = new List<PlatformState>(model.PlatformIds.Count);
        foreach (var platformId in model.PlatformIds) {
            if (platformController.Exist(platformId)) {
                platformStates.Add(platformController.ReadPlatformState(platformId));
            }
        }

        if (platformStates.Count == 0) {
            model.PendingLoadouts.Enqueue(loadout);
            return;
        }

        var freeSlotIndex = platformStates.FindIndex(state => !state.hasLoadout);
        var target = platformStates[freeSlotIndex >= 0 ? freeSlotIndex : 0];
        platformController.SetLoadout(target.platformId, loadout);
    }

    public void RegisterPlatform(int platformId) {
        if (!model.PlatformIds.Contains(platformId)) {
            model.PlatformIds.Add(platformId);
        }

        PlacePendingLoadouts();
    }

    public void UnregisterPlatform(int platformId) {
        model.PlatformIds.Remove(platformId);
    }

    private void PlacePendingLoadouts() {
        while (model.PendingLoadouts.Count > 0 && model.PlatformIds.Count > 0) {
            var loadout = model.PendingLoadouts.Dequeue();
            Place(loadout);
        }
    }
}
