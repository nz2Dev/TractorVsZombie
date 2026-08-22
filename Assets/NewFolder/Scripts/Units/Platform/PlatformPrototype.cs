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
    public RaycastMarker raycastMarkerPrefab;

    public PlatformPrototype(Vector3 position, PlatformConfig config, PlatformVisuals visualsPrefab, UnityVehicle vehiclePrefab, RamEffectPrototype ramPrototype, Vector3 loadoutOffset, CombatPrototype combatPrototype, RaycastMarker raycastMarkerPrefab) {
        this.position = position;
        this.config = config;
        this.visualsPrefab = visualsPrefab;
        this.vehiclePrefab = vehiclePrefab;
        this.ramPrototype = ramPrototype;
        this.loadoutOffset = loadoutOffset;
        this.combatPrototype = combatPrototype;
        this.raycastMarkerPrefab = raycastMarkerPrefab;
    }
}
