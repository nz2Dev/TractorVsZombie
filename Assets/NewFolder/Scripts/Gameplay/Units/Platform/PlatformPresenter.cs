using UnityEngine;

public class PlatformPresenter {

    private readonly PlatformView view;
    private readonly PlatformUI ui;

    public PlatformPresenter(PlatformView view, PlatformUI ui) {
        this.view = view;
        this.ui = ui;
    }

    public void Add(int platformId, Vector3 position, bool alie, PlatformPresentationPrototype prototype) {
        view.Show(platformId, position, prototype.visualsPrefab, alie);
        ui.Add(platformId, position, prototype.worldSpaceUIPrefab);
    }

    public void Remove(int platformId) {
        view.Hide(platformId);
        ui.Remove(platformId);
    }

    public void Update(int platformId, VehicleState vehicleState, HealthState healthState) {
        var vehiclePose = vehicleState.pose;
        view.SetPose(platformId, vehiclePose);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit(platformId);
        }
        ui.UpdateHealthBar(platformId, vehiclePose.position, healthState.health, healthState.maxHealth);
    }
}
