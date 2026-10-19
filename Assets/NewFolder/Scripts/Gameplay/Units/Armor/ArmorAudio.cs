using System.Collections.Generic;

using UnityEngine;

public class ArmorAudio {

    private readonly SoundManager soundManager;
    private readonly Dictionary<int, int> registry = new();

    public ArmorAudio(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Add(int armorId, Vector3 position, AudioClip engineLoopSFX) {
        registry[armorId] = soundManager.StartLoop(position, engineLoopSFX);
    }

    public void Remove(int armorId) {
        var loopId = registry[armorId];
        soundManager.StopLoop(loopId);
    }

    public void UpdateEngineSound(int armorId, Vector3 position, float gasThrottle) {
        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.5f + gasThrottle;
        var engineLoopId = registry[armorId];
        soundManager.UpdateLoop(engineLoopId, position, enginePitch, engineVolume);
    }
}
