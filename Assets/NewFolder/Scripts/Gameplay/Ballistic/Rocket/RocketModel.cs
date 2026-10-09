using Combat;

public class RocketModel {
    
    internal int Id { get; private set; }
    internal CombatId ShooterCombatId { get; private set; }
    internal bool ShooterIsAlie { get; }
    internal FlyPath Trajectory { get; private set; }
    internal RocketConfig Config { get; private set; }
    internal float LaunchTime { get; private set; }

    public RocketModel(int id, CombatId shooterCombatId, bool shooterIsAlie, float launchTime, FlyPath trajectory, RocketConfig config) {
        Id = id;
        ShooterCombatId = shooterCombatId;
        Trajectory = trajectory;
        Config = config;
        LaunchTime = launchTime;
        ShooterIsAlie = shooterIsAlie;
    }

    internal bool Landed { get; set; }

}