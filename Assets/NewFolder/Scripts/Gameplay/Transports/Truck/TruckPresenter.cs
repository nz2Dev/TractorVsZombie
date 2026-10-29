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

    public void Add(int truckId, Vector3 position, TruckPresentationPrototype prototype) {
        view.Show(truckId, position, prototype.visualsPrefab);
        ui.Add(truckId, position, prototype.worldSpaceUIPrefab);
        audio.Add(truckId, position, prototype);
    }

    public void Remove(int truckId) {
        view.Hide(truckId);
        ui.Remove(truckId);
        audio.Remove(truckId);
    }

    public void Update(int truckId, VehicleState vehicleState, HealthState healthState, RamState ramState) {
        var vehiclePose = vehicleState.pose;
        view.SetPose(truckId, vehiclePose);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit(truckId);
        }
        
        ui.UpdateHealthBar(truckId, vehiclePose.position, healthState.health, healthState.maxHealth);
        audio.UpdateEngineSound(truckId, vehiclePose.position, vehicleState.motorRev);
        if (ramState.receivedContacts > 0) {
            audio.PlayRamImpact(truckId, vehiclePose.position, ramState.receivedContacts);
        }
    }
}
