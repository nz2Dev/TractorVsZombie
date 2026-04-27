using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Configs/ProjectileConfig")]
public class ProjectileConfig : ScriptableObject {
    public int damage;
    public float speed;
    public float lifetime;
    public AudioClip[] shootAudioClips;
    public ProjectileStyle style;
}
