using System.Collections.Generic;

using UnityEngine;

public class ArmorAudio {
    
    private readonly SoundManager soundManager;
    
    private Dictionary<int, ArmorAudioModel> registry = new();

    public ArmorAudio(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Add(int armorId, Vector3 position, ArmorAudioPrototype audioPrototype) {
        registry[armorId] = new ArmorAudioModel {
            Config = audioPrototype.config,
            EngineLoopId = soundManager.StartLoop(position, audioPrototype.config.engineLoopSFX)
        };
    }

    public void Remove(int armorId) {
        registry.Remove(armorId, out var model);
        soundManager.StopLoop(model.EngineLoopId);
    }

    public void UpdateSound(int armorId, Vector3 position, float gasThrottle) {
        var model = registry[armorId];
        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.5f + gasThrottle;
        soundManager.UpdateLoop(model.EngineLoopId, position, enginePitch, engineVolume);
    }
}