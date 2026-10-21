using UnityEngine;

public class TruckPresenter {
    private readonly TruckAudio audio;
    private readonly TruckView view;
    private readonly TruckUI ui;

    public TruckPresenter(TruckAudio audio, TruckView view, TruckUI ui) {
        this.audio = audio;
        this.view = view;
        this.ui = ui;
    }

    public void Add(int truckId, Vector3 position, bool alie, TruckPresentationPrototype prototype) {
        view.Show(truckId, position, alie, prototype.visualsPrefab);
        ui.Add(truckId, position, prototype.worldSpaceUIPrefab);
        audio.Add(truckId, position, prototype);
    }

    public void Remove(int truckId) {
        view.Hide(truckId);
        ui.Remove(truckId);
        audio.Remove(truckId);
    }

    public void Update(int truckId, VehiclePose vehicleState, float gasThrottle, HealthState healthState, RamState ramState) {
        view.SetPose(truckId, vehicleState);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit(truckId);
        }
        ui.UpdateHealthBar(truckId, vehicleState.position, healthState.health, healthState.maxHealth);
        audio.UpdateEngineSound(truckId, vehicleState.position, gasThrottle);
        if (ramState.receivedContacts > 0) {
            audio.PlayRamImpact(truckId, vehicleState.position, ramState.receivedContacts);
        }
    }
}
