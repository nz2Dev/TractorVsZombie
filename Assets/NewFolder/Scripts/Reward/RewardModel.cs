using System.Collections.Generic;

using UnityEngine;

public enum RewardType {
    Points,
    Weapon,
}

public class RewardModel {
    public int Id { get; set; }
    public int SpatialId { get; set; }
    public Vector3 Position { get; set; }
    public RewardType RewardType { get; set; }
    public WeaponConfig WeaponConfig { get; set; } 
}