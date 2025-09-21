using UnityEngine;

public class SoundManager : MonoBehaviour {
    
    private AudioSource[] audioSources;

    private void Awake() {
        audioSources = GetComponentsInChildren<AudioSource>();
    }

    public void PlayEffect(Vector3 position, AudioClip[] audioClipVariants) {
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