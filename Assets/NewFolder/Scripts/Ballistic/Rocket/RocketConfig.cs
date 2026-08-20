using Compatibility;

using UnityEngine;

[CreateAssetMenu(fileName = "RocketConfig", menuName = "Configs/RocketConfig")]
public class RocketConfig : ScriptableObject {
    public AudioClip[] launchEffectClips;
    public AudioClip[] explodeEffectClips;
    public FlyShape flyShape;
    public float flyDuration;
    public int damage;
    public float explosionRadius; // NOTE: is duplicating explosion data
    public ExplosionData explosionData = new() {
        force = 10,
        radius = 5,
        upwardModifier = 1
    };
}
