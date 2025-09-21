using UnityEngine;

[CreateAssetMenu(fileName = "RocketLauncherConfig", menuName = "RocketLauncherConfig", order = 0)]
public class RocketLauncherConfig : ScriptableObject {
    public float radius = 10;
    public float launchIntervalSec = 1;
    public AudioClip[] launchEffectClips;
    public AudioClip[] explodeEffectClips;
    public float rocketAmplitude = 10;
    public float flyDuration = 4;
    public int damage = 1;
}