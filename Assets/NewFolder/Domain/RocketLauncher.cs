using UnityEngine;

public struct RocketTrajectory { //
    public Vector3 launchPoint;
    public Vector3 landPoint;
    public float flyDuration;
    public AudioClip[] launchEffectClips;
    public AudioClip[] explodeEffectClips;
    public float height; //
}

public class RocketLauncher {
    
    private readonly RocketLauncherConfig config;
    private float lastLaunchTime = float.NegativeInfinity;

    public RocketLauncher(int id, Vector3 position, RocketLauncherConfig config) {
        Id = id;
        Position = position;
        this.config = config;
    }

    public int Id { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 AimPoint { get; private set; }

    public RocketLauncherVisuals Visuals => config.visualsPrefab;
    public float RocketAmplitude => config.rocketAmplitude;
    public float RocketFlyDuration => config.flyDuration;
    public AudioClip[] RocketLaunchEffects => config.launchEffectClips;
    public AudioClip[] ExplodeEffectClips => config.explodeEffectClips;
    public int RocketDamage => config.damage;
    public float Radius => config.radius;

    public void Translate(Vector3 position) {
        Position = position;
    }

    public void Aim(Vector3 point) {
        AimPoint = point;
    }

    public bool Launch(float time, out RocketTrajectory trajectory) {
        if (lastLaunchTime + config.launchIntervalSec > time) {
            trajectory = default;
            return false;
        }
        
        lastLaunchTime = time;
        trajectory = new RocketTrajectory {
            launchPoint = Position,
            landPoint = AimPoint,
            height = config.rocketAmplitude,
            flyDuration = 1,
            launchEffectClips = config.launchEffectClips,
            explodeEffectClips = config.explodeEffectClips
        };
        return true;
    }

}