using System;

using UnityEngine;

public class ArmorView {

    private readonly SoundManager soundManager;

    private ArmorVisuals visuals;
    private int sfxLoopId;

    public ArmorView(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Show(Vector3 position, ArmorVisuals prefab, AudioClip engineSFX) {
        visuals = GameObject.Instantiate(prefab, position, Quaternion.identity);
        sfxLoopId = soundManager.StartLoop(position, engineSFX);
    }

    public void Hide() {
        if (visuals != null) {
            visuals.DestroySelf();
            visuals = null;
        }
        soundManager.StopLoop(sfxLoopId);
    }

    public void UpdatePose(VehicleState vehicleState) {
        if (visuals == null) return;
        
        visuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        visuals.SetFrontAxis(vehicleState.frontAxis);
        visuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void UpdateSound(float gasThrottle) {
        if (visuals == null) return;

        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.5f + gasThrottle;
        soundManager.UpdateLoop(sfxLoopId, visuals.transform.position, enginePitch, engineVolume);
    }
}
