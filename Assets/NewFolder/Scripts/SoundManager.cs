using System.Collections.Generic;

using UnityEngine;

public class SoundManager : MonoBehaviour {
    
    private AudioSource[] audioSources;

    private int dynamicIdCounter;
    private Dictionary<int, AudioSource> loopSources;

    private void Awake() {
        audioSources = GetComponentsInChildren<AudioSource>();
        loopSources = new ();
    }

    public int StartLoop(Vector3 position, AudioClip audioClip) {
        var loopId = dynamicIdCounter++;
        var audioSource = Object.Instantiate(audioSources[0]);
        loopSources[loopId] = audioSource;
        audioSource.transform.position = position;
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
        return loopId;
    }

    public void UpdateLoop(int loopId, Vector3 position, float pitch, float volume) {
        var audioSource = loopSources[loopId];
        audioSource.transform.position = position;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
    }

    public void PlayEffect(Vector3 position, params AudioClip[] audioClipVariants) {
        var audioSource = GetNextNonPlaying();
        if (audioSource == null) {
            audioSource = GetLongestPlaying();
        }

        audioSource.transform.position = position;
        audioSource.pitch = Random.Range(0.8f, 1.4f);
        audioSource.clip = SelectRandom(audioClipVariants);
        audioSource.Play();
    }

    private AudioSource GetNextNonPlaying() {
        for (int i = 0; i < audioSources.Length; i++) {
            var audioSource = audioSources[i];
            if (!audioSource.isPlaying) {
                return audioSource;
            }
        }
        return null;
    }

    private AudioSource GetLongestPlaying() {
        float longestTime = -1;
        var longestIndex = -1;
        for (int i = 0; i < audioSources.Length; i++) {
            var audioSource = audioSources[i];
            if (audioSource.time > longestTime) {
                longestTime = audioSource.time;
                longestIndex = i;
            }
        }
        return audioSources[longestIndex];
    }

    private AudioClip SelectRandom(AudioClip[] clips) {
        return clips[Random.Range(0, clips.Length)];
    }

}