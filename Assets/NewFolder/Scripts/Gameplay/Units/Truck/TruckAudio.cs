using System.Collections.Generic;

using UnityEngine;

public class TruckAudio {
    private readonly SoundManager soundManager;
    private readonly Dictionary<int, int> registry = new();

    public TruckAudio(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Add(int truckId, Vector3 position, AudioClip engineLoopSFX) {
        registry[truckId] = soundManager.StartLoop(position, engineLoopSFX);
    }

    public void Remove(int truckId) {
        var loopId = registry[truckId];
        registry.Remove(truckId);
        soundManager.StopLoop(loopId);
    }

    public void UpdateEngineSound(int truckId, Vector3 position, float gasThrottle) {
        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.1f + gasThrottle;
        soundManager.UpdateLoop(registry[truckId], position, enginePitch, engineVolume);
    }
}
