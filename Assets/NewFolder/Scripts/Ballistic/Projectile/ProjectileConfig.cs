using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Configs/ProjectileConfig")]
public class ProjectileConfig : ScriptableObject {
    public int damage;
    public float speed;
    public float lifetime;
    public AudioClip[] shootAudioClips;
    public ProjectileStyle style;
    [Space]
    public AudioClip[] impactAudioClips;
    public ParticleSystem impactParticlesPrefab;
    [Space]
    public AudioClip[] metalImpactAudioClips;
    public ParticleSystem metalImpactParticlesPrefab;
    [Space]
    public AudioClip[] softImpactAudioClips;
    public ParticleSystem softImpactParticlesPrefab;
}
