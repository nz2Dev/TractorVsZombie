using System;
using System.Collections.Generic;

using UnityEngine;

public class ArmorView {

    private readonly SoundManager soundManager;

    private Dictionary<int, ArmorVisuals> visualsRegistry = new();
    private Dictionary<int, int> sfxLoopRegistry = new();

    public ArmorView(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Show(int armorId, Vector3 position, ArmorVisuals prefab, AudioClip engineSFX) {
        visualsRegistry[armorId] = GameObject.Instantiate(prefab, position, Quaternion.identity);
        sfxLoopRegistry[armorId] = soundManager.StartLoop(position, engineSFX);
    }

    public void Hide(int armorId) {
        visualsRegistry.Remove(armorId, out var visuals);
        visuals.DestroySelf();
        sfxLoopRegistry.Remove(armorId, out var sfxLoopId);
        soundManager.StopLoop(sfxLoopId);
    }

    public void UpdatePose(int armorId, VehicleState vehicleState) {
        var visuals = visualsRegistry[armorId];
        visuals.SetPositionAndRotation(vehicleState.position, vehicleState.rotation);
        visuals.SetFrontAxis(vehicleState.frontAxis);
        visuals.SetRearAxis(vehicleState.rearAxis);
    }

    public void UpdateSound(int armorId, float gasThrottle) {
        var visuals = visualsRegistry[armorId];
        var sfxLoopId = sfxLoopRegistry[armorId];

        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.5f + gasThrottle;
        soundManager.UpdateLoop(sfxLoopId, visuals.transform.position, enginePitch, engineVolume);
    }
}
