public class RocketModel {
    
    internal int Id { get; private set; }
    internal int ShooterId { get; private set; }
    internal FlyPath Trajectory { get; private set; }
    internal RocketConfig Config { get; private set; }
    internal float LaunchTime { get; private set; }
    
    public RocketModel(int id, int shooterId, float launchTime, FlyPath trajectory, RocketConfig config) {
        Id = id;
        ShooterId = shooterId;
        Trajectory = trajectory;
        Config = config;
        LaunchTime = launchTime;
    }

    internal bool Landed { get; set; }

}