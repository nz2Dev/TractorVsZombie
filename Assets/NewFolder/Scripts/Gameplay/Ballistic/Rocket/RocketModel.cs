
public class RocketModel {
    
    internal int Id { get; private set; }
    internal FactionId ShooterFactionId { get; private set; }
    internal bool ShooterIsAlie { get; }
    internal FlyPath Trajectory { get; private set; }
    internal RocketConfig Config { get; private set; }
    internal float LaunchTime { get; private set; }

    public RocketModel(int id, FactionId shooterFactionId, bool shooterIsAlie, float launchTime, FlyPath trajectory, RocketConfig config) {
        Id = id;
        ShooterFactionId = shooterFactionId;
        Trajectory = trajectory;
        Config = config;
        LaunchTime = launchTime;
        ShooterIsAlie = shooterIsAlie;
    }

    internal bool Landed { get; set; }

}
