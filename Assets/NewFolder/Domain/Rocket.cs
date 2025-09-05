using UnityEngine;

public class Rocket {

    private readonly float launchTime;
    private readonly float flyDuration;
    
    public Rocket(int id, RocketTrajectory trajectory, float launchTime, float flyDuration) {
        Id = id;
        Trajectory = trajectory;
        this.launchTime = launchTime;
        this.flyDuration = flyDuration;
    }

    public int Id { get; private set; }
    public RocketTrajectory Trajectory { get; private set; }
    public bool Landed { get; private set; } = false;

    public bool ForwardLandingTime(float time) {
        if (Landed)
            return false;

        Landed = launchTime + flyDuration < time;
        return Landed;
    }
    
}