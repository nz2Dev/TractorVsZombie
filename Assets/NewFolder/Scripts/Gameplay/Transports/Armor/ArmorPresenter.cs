using System.Collections.Generic;

using UnityEngine;

public class ArmorPresenter {

    private readonly ArmorAudio audio;
    private readonly ArmorView view;
    private readonly ArmorUI ui;

    public ArmorPresenter(ArmorAudio audio, ArmorView view, ArmorUI ui) {
        this.audio = audio;
        this.view = view;
        this.ui = ui;
    }

    public void Add(int armorId, Vector3 position, ArmorPresentationPrototype prototype) {
        view.Show(armorId, position, prototype.visualsPrefab);
        ui.Add(armorId, position, prototype.worldSpaceUIPrefab);
        audio.Add(armorId, position, prototype);
    }

    public void Remove(int armorId) {
        view.Hide(armorId);
        ui.Remove(armorId);
        audio.Remove(armorId);
    }

    public void Update(int armorId, VehicleState vehicleState, HealthState healthState, RamState ramState) {
        var vehiclePose = vehicleState.pose;
        view.SetPose(armorId, vehiclePose);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit(armorId);
        }
        ui.UpdateHealthBar(armorId, vehiclePose.position, healthState.health, healthState.maxHealth);
        audio.UpdateEngineSound(armorId, vehiclePose.position, vehicleState.motorRev);
        if (ramState.receivedContacts > 0) {
            audio.PlayRamImpact(armorId, vehiclePose.position, ramState.receivedContacts);
        }
    }
}
