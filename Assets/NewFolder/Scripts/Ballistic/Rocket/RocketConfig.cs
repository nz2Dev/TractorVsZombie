using UnityEngine;

[CreateAssetMenu(fileName = "RocketConfig", menuName = "Configs/RocketConfig")]
public class RocketConfig : ScriptableObject {
    public AudioClip[] launchEffectClips;
    public AudioClip[] explodeEffectClips;
    public AnimationCurve flyCurve;
    public float amplitude;
    public float flyDuration;
    public int damage;
    public RocketVisuals visualsPrefab;
}
