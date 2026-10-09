using System.Collections.Generic;

using UnityEngine;

// Is a presenation layer subsystem that handles work related to playing audio in different ways
// Should be used by presentation components such as Views.
// ***
// I keep it in separate folder, to break folder coupling with GameBootstrapper
// future is still to be decided
// ***
// It doesn't fit inside Local/ "packages" semantic as it's not a library in its sense, it's tight to the application we develop
public class SoundManager : MonoBehaviour {
    
    [SerializeField] private int effectSourcesCount = 30;

    private AudioSource audioSourcePrefab;

    private int dynamicIdCounter;
    private Dictionary<int, AudioSource> loopSources;
    private List<AudioSource> effectSources;

    private void Awake() {
        audioSourcePrefab = GetComponentInChildren<AudioSource>();
        loopSources = new ();
        effectSources = new (30);

        for (int i = 0; i < effectSourcesCount; i++) {
            var effectSource = Instantiate(audioSourcePrefab, transform);
            effectSources.Add(effectSource);
        }
    }

    public int StartLoop(Vector3 position, AudioClip audioClip) {
        var loopId = dynamicIdCounter++;
        var audioSource = Object.Instantiate(audioSourcePrefab);
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

    public void StopLoop(int loopId) {
        var audioSource = loopSources[loopId];
        GameObject.Destroy(audioSource.gameObject);
        loopSources.Remove(loopId);
    }

    public void PlayEffectDelayed(Vector3 position, float delay, params AudioClip[] audioClipVariants) {
        var audioSource = GetNextNonPlaying();
        if (audioSource == null) {
            audioSource = GetLongestPlaying();
        }

        audioSource.transform.position = position;
        audioSource.pitch = Random.Range(0.8f, 1.4f);
        audioSource.clip = SelectRandom(audioClipVariants);
        audioSource.PlayDelayed(delay);
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
        for (int i = 0; i < effectSources.Count; i++) {
            var audioSource = effectSources[i];
            if (!audioSource.isPlaying) {
                return audioSource;
            }
        }
        return null;
    }

    private AudioSource GetLongestPlaying() {
        float longestTime = -1;
        var longestIndex = -1;
        for (int i = 0; i < effectSources.Count; i++) {
            var audioSource = effectSources[i];
            if (audioSource.time > longestTime) {
                longestTime = audioSource.time;
                longestIndex = i;
            }
        }
        return effectSources[longestIndex];
    }

    public static AudioClip SelectRandom(AudioClip[] clips) {
        return clips[Random.Range(0, clips.Length)];
    }

}