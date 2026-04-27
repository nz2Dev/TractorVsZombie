using UnityEngine;

public class RewardModel : IPositionSource {
    public RewardModel(int id, Vector3 position, RewardType rewardType, LoadoutPrototype loadoutPrototype) {
        Id = id;
        Position = position;
        RewardType = rewardType;
        LoadoutPrototype = loadoutPrototype;
    }

    public int Id { get; }
    public Vector3 Position { get; }
    public RewardType RewardType { get;}
    public LoadoutPrototype LoadoutPrototype { get; }

}