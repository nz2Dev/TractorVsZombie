
using UnityEngine;

[System.Serializable]
public struct PlatformPrototype {
    public Vector3 position;
    public PlatformVisuals visualsPrefab;
    public VehiclePrototype vehiclePrototype;
    public CarryingPrototype carryingPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public WorldSpaceUI worldSpaceUIPrefab;

    public PlatformPrototype(Vector3 position, PlatformVisuals visualsPrefab, VehiclePrototype vehiclePrototype, CarryingPrototype carryingPrototype, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.visualsPrefab = visualsPrefab;
        this.vehiclePrototype = vehiclePrototype;
        this.carryingPrototype = carryingPrototype;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
