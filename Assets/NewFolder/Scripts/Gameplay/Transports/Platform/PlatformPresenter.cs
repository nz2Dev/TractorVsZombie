using UnityEngine;

public class PlatformPresenter {

    private readonly PlatformView view;
    private readonly PlatformUI ui;

    public PlatformPresenter(PlatformView view, PlatformUI ui) {
        this.view = view;
        this.ui = ui;
    }

    public void Add(int platformId, Vector3 position, PlatformPresentationPrototype prototype) {
        view.Show(platformId, position, prototype.visualsPrefab);
        ui.Add(platformId, position, prototype.displayPrefab);
    }

    public void Remove(int platformId) {
        view.Hide(platformId);
        ui.Remove(platformId);
    }

    public void Update(int platformId, VehicleState vehicleState, CarryingState carryingState) {
        var vehiclePose = vehicleState.pose;
        view.SetPose(platformId, vehiclePose);

        ui.UpdatePose(platformId, vehiclePose.position, vehiclePose.rotation * Vector3.forward);
        ui.UpdateDurability(platformId, carryingState);
        if (carryingState.hasLoadout)
            ui.UpdateHealthBar(platformId, carryingState.loadoutState.healthState);
        else
            ui.HideHealthBar(platformId);
    }
}
