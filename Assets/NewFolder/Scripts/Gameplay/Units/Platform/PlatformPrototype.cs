
using UnityEngine;

[System.Serializable]
public struct PlatformPrototype {
    public Vector3 position;
    public PlatformVisuals visualsPrefab;
    public VehiclePrototype vehiclePrototype;
    public Vector3 loadoutOffset;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public WorldSpaceUI worldSpaceUIPrefab;

    public PlatformPrototype(Vector3 position, PlatformVisuals visualsPrefab, VehiclePrototype vehiclePrototype, Vector3 loadoutOffset, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.visualsPrefab = visualsPrefab;
        this.vehiclePrototype = vehiclePrototype;
        this.loadoutOffset = loadoutOffset;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
