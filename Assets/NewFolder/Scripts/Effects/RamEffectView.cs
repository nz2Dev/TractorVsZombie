using System.Collections.Generic;

using UnityEditorInternal;

using UnityEngine;

public class RamEffectView {

    internal class State {
        public float lastImpactTime;
    }
    
    private Dictionary<int, AudioSource> audioSourceRegistry = new ();
    private Dictionary<int, State> stateRegistry = new ();

    public RamEffectView(SoundManager soundManager) {
        
    }

    public void AddEffect(int entityId, AudioSource audioSourcePrefab) {
        audioSourceRegistry[entityId] = GameObject.Instantiate(audioSourcePrefab);
        stateRegistry[entityId] = new State();
    }

    public void ShowImpact(int entityId, Vector3 position, int times, AudioClip[] audioClips) {
        var impactLimit = Random.Range(1, 1);
        var maxImpacts = Mathf.Min(times, impactLimit);
        var audioSource = audioSourceRegistry[entityId];
        audioSource.transform.position = position;
        var state = stateRegistry[entityId];
        if (Time.time - state.lastImpactTime > 0f) {
            state.lastImpactTime = Time.time;
            for (int i = 0; i < maxImpacts; i++) {
                audioSource.PlayOneShot(SelectRandom(audioClips));
            }
        }
    }   

    private static AudioClip SelectRandom(AudioClip[] clips) {
        return clips[Random.Range(0, clips.Length)];
    }

}