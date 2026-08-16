using System.Collections.Generic;

using UnityEditorInternal;

using UnityEngine;

public class RamEffectView {

    private Dictionary<int, AudioSource> audioSourceRegistry = new ();

    public RamEffectView(SoundManager soundManager) {
    }

    public void AddEffect(int entityId, AudioSource audioSourcePrefab) {
        audioSourceRegistry[entityId] = GameObject.Instantiate(audioSourcePrefab);
    }

    public void ShowImpact(int entityId, Vector3 position, int times, AudioClip[] audioClips) {
        var impactLimit = 3;
        var maxImpacts = Mathf.Min(times, impactLimit);
        var audioSource = audioSourceRegistry[entityId];
        audioSource.transform.position = position;
        for (int i = 0; i < maxImpacts; i++) {
            audioSource.PlayOneShot(SelectRandom(audioClips));
        }
    }   

    private static AudioClip SelectRandom(AudioClip[] clips) {
        return clips[Random.Range(0, clips.Length)];
    }

}