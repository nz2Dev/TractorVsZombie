using UnityEngine;

public enum RewardType {
    Points,
    Loadout,
}

public class RewardModel : IPositionSource {
    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public RewardType RewardType { get; set; }
    public LoadoutConfig LoadoutConfig { get; set; }
}