using System.Collections.Generic;

using UnityEngine;

public class ArmorAudio {
    
    private readonly SoundManager soundManager;
    
    private Dictionary<int, int> sfxLoopRegistry = new();

    public ArmorAudio(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Add(int armorId, Vector3 position, ArmorAudioPrototype audioPrototype) {
        sfxLoopRegistry[armorId] = soundManager.StartLoop(position, audioPrototype.engineLoopSFX);
    }

    public void Remove(int armorId) {
        sfxLoopRegistry.Remove(armorId, out var sfxLoopId);
        soundManager.StopLoop(sfxLoopId);
    }

    public void UpdateSound(int armorId, Vector3 position, float gasThrottle) {
        var sfxLoopId = sfxLoopRegistry[armorId];
        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.5f + gasThrottle;
        soundManager.UpdateLoop(sfxLoopId, position, enginePitch, engineVolume);
    }
}