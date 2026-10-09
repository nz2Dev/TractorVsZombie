using UnityEngine;

public class ProjectileSource : MonoBehaviour {
    
    [Inline] [SerializeField] private ProjectileConfig config;
    [SerializeField] private AudioSource shootAudioSourcePrefab;
    [SerializeField] private AudioSource crashAudioSourcePrefab;

    public ProjectilePrototype Get() {
        return new ProjectilePrototype {
            config = config,
            shootAudioSourcePrefab = shootAudioSourcePrefab,
            crashAudioSourcePrefab = crashAudioSourcePrefab
        };
    }

}