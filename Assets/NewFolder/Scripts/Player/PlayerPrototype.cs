using UnityEngine;

public struct PlayerPrototype {
    public PlayerConfig config;
    public AimVisuals aimVisualsPrefab; // is a view dependency, should the view has its mechanism for injections from scene/prefab/config?
    public TruckPrototype initTruckPrototype;
    public PlatformPrototype pickupPlatformPrototype;
    public LoadoutPrototype[] initLoadoutPrototypes;
}