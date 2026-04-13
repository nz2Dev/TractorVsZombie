using UnityEngine;

public struct RewardState {
    public Vector3 Position { get; set; }
    public RewardType RewardType { get; set; }
    public GameObject BrokenArmorVisualsPrefab { get; set; }
    public WeaponConfig WeaponConfig { get; set; } 
    public Vector3 WeaponOffset { get; set; }
}