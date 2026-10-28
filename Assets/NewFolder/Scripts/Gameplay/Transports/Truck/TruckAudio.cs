using System.Collections.Generic;

using UnityEngine;

public class TruckAudio {
    private struct AudioHolder {
        public int LoopId;
        public AudioSource RamImpactSource;
        public AudioClip[] RamImpactClips;
    }

    private readonly SoundManager soundManager;
    private readonly Dictionary<int, AudioHolder> registry = new();

    public TruckAudio(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Add(int truckId, Vector3 position, TruckPresentationPrototype prototype) {
        registry[truckId] = new AudioHolder {
            LoopId = soundManager.StartLoop(position, prototype.engineLoopSFX),
            RamImpactSource = GameObject.Instantiate(prototype.ramImpactAudioSourcePrefab),
            RamImpactClips = prototype.ramImpactSFX
        };
    }

    public void Remove(int truckId) {
        var holder = registry[truckId];
        registry.Remove(truckId);
        soundManager.StopLoop(holder.LoopId);
        GameObject.Destroy(holder.RamImpactSource.gameObject);
    }

    public void UpdateEngineSound(int truckId, Vector3 position, float gasThrottle) {
        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.1f + gasThrottle;
        soundManager.UpdateLoop(registry[truckId].LoopId, position, enginePitch, engineVolume);
    }
    
    public void PlayRamImpact(int truckId, Vector3 position, int contacts) {
        var holder = registry[truckId];
        holder.RamImpactSource.transform.position = position;
        for (var i = 0; i < Mathf.Min(contacts, 3); i++)
            holder.RamImpactSource.PlayOneShot(
                holder.RamImpactClips[Random.Range(0, holder.RamImpactClips.Length)]);
    }

}
