using UnityEngine;

public class RewardModel : IPositionSource {
    
    public RewardModel(int id, Vector3 position, RewardPayload payload) {
        Id = id;
        Position = position;
        Payload = payload;
    }

    public int Id { get; }
    public Vector3 Position { get; }
    public RewardPayload Payload { get; }

}