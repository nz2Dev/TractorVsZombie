using System;

using UnityEngine;

public class TruckView {

    private readonly SoundManager soundManager;
    
    private TruckVisuals visuals;
    private int sfxLoopId;

    public TruckView(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Show(Vector3 position, TruckVisuals prefab, AudioClip engineSFX) {
        visuals = GameObject.Instantiate(prefab, position, Quaternion.identity);
        sfxLoopId = soundManager.StartLoop(position, engineSFX);
    }

    public void UpdatePose(VehicleState vehicleState) {
        visuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        visuals.SetFrontAxis(vehicleState.frontAxis);
        visuals.SetRearAxis(vehicleState.rearAxis);
    }

    internal void UpdateSound(float gasThrottle) {
        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.1f + gasThrottle;
        soundManager.UpdateLoop(sfxLoopId, visuals.transform.position, enginePitch, engineVolume);
    }
}