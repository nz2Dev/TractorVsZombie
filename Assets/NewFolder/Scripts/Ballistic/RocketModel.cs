using UnityEngine;

public struct RocketTrajectory {
    public Vector3 launchPoint;
    public Vector3 landPoint;
}

public class RocketModel {
    
    internal int Id { get; private set; }
    internal int ShooterId { get; private set; }
    internal RocketTrajectory Trajectory { get; private set; }
    internal RocketConfig Config { get; private set; }
    internal float LaunchTime { get; private set; }
    internal bool Landed { get; private set; }

    public RocketModel(int id, int shooterId, float launchTime, RocketTrajectory trajectory, RocketConfig config) {
        Id = id;
        ShooterId = shooterId;
        Trajectory = trajectory;
        Config = config;
        LaunchTime = launchTime;
        Landed = false;
    }

    public bool ForwardLandingTime(float time) {
        if (Landed)
            return false;

        Landed = LaunchTime + 1 < time;
        return Landed;
    }
}