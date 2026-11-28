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
        // todo remove truckId key from view and audio
        audio.Add(truckId, position, prototype);
        view.Show(truckId, position, prototype.visualsPrefab);
        ui.Add(position, prototype.statsDisplayPrefab, prototype.outlineDisplayPrefab);
    }

    public void Remove(int truckId) {
        view.Hide(truckId);
        ui.Remove();
        audio.Remove(truckId);
    }

    public void Update(int truckId, VehicleState vehicleState, HealthState healthState, RamState ramState, AttachmentState attachmentState) {
        var vehiclePose = vehicleState.pose;
        view.SetPose(truckId, vehiclePose);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit(truckId);
        }
        
        ui.UpdatePose(vehiclePose.position, vehiclePose.rotation * Vector3.forward);
        ui.UpdateHealthBar(healthState.health, healthState.maxHealth);
        
        ui.UpdateAimPoint(attachmentState.weaponState.aimPoint);
        ui.SetBulletIcon(attachmentState.weaponState.category.bulletImage);
        ui.SetAimIcon(attachmentState.weaponState.category.bulletImage);
        ui.ShowBulletIcon(true);

        audio.UpdateEngineSound(truckId, vehiclePose.position, vehicleState.motorRev);
        if (ramState.receivedContacts > 0) {
            audio.PlayRamImpact(truckId, vehiclePose.position, ramState.receivedContacts);
        }
    }
}
