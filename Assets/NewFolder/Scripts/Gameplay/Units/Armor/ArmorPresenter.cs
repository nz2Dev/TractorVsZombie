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

    public void Add(int armorId, Vector3 position, bool alie, ArmorPresentationPrototype prototype) {
        view.Show(armorId, position, alie, prototype.visualsPrefab);
        ui.Add(armorId, position, prototype.worldSpaceUIPrefab);
        audio.Add(armorId, position, prototype);
    }

    public void Remove(int armorId) {
        view.Hide(armorId);
        ui.Remove(armorId);
        audio.Remove(armorId);
    }

    public void Update(int armorId, VehicleState vehicleState, float gasThrottle, HealthState healthState, RamState ramState) {
        view.SetPose(armorId, vehicleState);
        if (healthState.damageResult.HasValue) {
            view.ShowTakeHit(armorId);
        }
        ui.UpdateHealthBar(armorId, vehicleState.position, healthState.health, healthState.maxHealth);
        audio.UpdateEngineSound(armorId, vehicleState.position, gasThrottle);
        if (ramState.receivedContacts > 0) {
            audio.PlayRamImpact(armorId, vehicleState.position, ramState.receivedContacts);
        }
    }
}
