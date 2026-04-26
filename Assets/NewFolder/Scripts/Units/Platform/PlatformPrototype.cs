using UnityEngine;

[System.Serializable]
public struct PlatformPrototype {
    public Vector3 position;
    public PlatformConfig config;
    public PlatformVisuals visualsPrefab;
    public VehiclePhysics physicsPrefab;
    public RamEffectPrototype ramPrototype;
    public Vector3 loadoutOffset;
}
