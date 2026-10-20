using System.Collections.Generic;

using UnityEngine;

public class ArmorAudio {

    private struct AudioHolder {
        public int LoopId;
        public AudioSource RamImpactSource;
        public AudioClip[] RamImpactClips;
    }

    private readonly SoundManager soundManager;
    private readonly Dictionary<int, AudioHolder> registry = new();

    public ArmorAudio(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void Add(int armorId, Vector3 position, ArmorPresentationPrototype prototype) {
        registry[armorId] = new AudioHolder {
            LoopId = soundManager.StartLoop(position, prototype.engineLoopSFX),
            RamImpactSource = GameObject.Instantiate(prototype.ramImpactAudioSourcePrefab),
            RamImpactClips = prototype.ramImpactSFX
        };
    }

    public void Remove(int armorId) {
        var holder = registry[armorId];
        registry.Remove(armorId);
        soundManager.StopLoop(holder.LoopId);
        GameObject.Destroy(holder.RamImpactSource.gameObject);
    }

    public void UpdateEngineSound(int armorId, Vector3 position, float gasThrottle) {
        var enginePitch = 0.5f + gasThrottle;
        var engineVolume = 0.5f + gasThrottle;
        var engineLoopId = registry[armorId].LoopId;
        soundManager.UpdateLoop(engineLoopId, position, enginePitch, engineVolume);
    }

    public void PlayRamImpact(int armorId, Vector3 position, int contacts) {
        var holder = registry[armorId];
        holder.RamImpactSource.transform.position = position;
        for (var i = 0; i < Mathf.Min(contacts, 3); i++) {
            holder.RamImpactSource.PlayOneShot(
                holder.RamImpactClips[Random.Range(0, holder.RamImpactClips.Length)]);
        }
    }

}
