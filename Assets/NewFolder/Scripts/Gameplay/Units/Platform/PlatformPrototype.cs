
using UnityEngine;

[System.Serializable]
public struct PlatformPrototype {
    public Vector3 position;
    public PlatformConfig config;
    public PlatformVisuals visualsPrefab;
    public UnityVehicle vehiclePrefab;
    public RamEffectPrototype ramPrototype;
    public Vector3 loadoutOffset;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public RaycastMarker raycastMarkerPrefab;
    public WorldSpaceUI worldSpaceUIPrefab;

    public PlatformPrototype(Vector3 position, PlatformConfig config, PlatformVisuals visualsPrefab, UnityVehicle vehiclePrefab, RamEffectPrototype ramPrototype, Vector3 loadoutOffset, HealthPrototype healthPrototype, FactionPrototype factionPrototype, RaycastMarker raycastMarkerPrefab, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.config = config;
        this.visualsPrefab = visualsPrefab;
        this.vehiclePrefab = vehiclePrefab;
        this.ramPrototype = ramPrototype;
        this.loadoutOffset = loadoutOffset;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.raycastMarkerPrefab = raycastMarkerPrefab;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
