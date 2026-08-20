using Combat;

using UnityEngine;

[System.Serializable]
public struct PlatformPrototype {
    public Vector3 position;
    public PlatformConfig config;
    public PlatformVisuals visualsPrefab;
    public UnityVehicle vehiclePrefab;
    public RamEffectPrototype ramPrototype;
    public Vector3 loadoutOffset;
    public CombatPrototype combatPrototype;
}
