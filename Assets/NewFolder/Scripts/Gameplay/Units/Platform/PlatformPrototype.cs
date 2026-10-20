
using UnityEngine;

[System.Serializable]
public struct PlatformPrototype {
    public Vector3 position;
    public PlatformConfig config;
    public PlatformVisuals visualsPrefab;
    public UnityVehicle vehiclePrefab;
    public Vector3 loadoutOffset;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public WorldSpaceUI worldSpaceUIPrefab;

    public PlatformPrototype(Vector3 position, PlatformConfig config, PlatformVisuals visualsPrefab, UnityVehicle vehiclePrefab, Vector3 loadoutOffset, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.config = config;
        this.visualsPrefab = visualsPrefab;
        this.vehiclePrefab = vehiclePrefab;
        this.loadoutOffset = loadoutOffset;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
