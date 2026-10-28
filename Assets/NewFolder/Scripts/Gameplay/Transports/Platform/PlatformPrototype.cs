
using UnityEngine;

[System.Serializable]
public struct PlatformPrototype {
    public Vector3 position;
    public PlatformPresentationPrototype presentationPrototype;
    public VehiclePrototype vehiclePrototype;
    public CarryingPrototype carryingPrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;

    public PlatformPrototype(Vector3 position, PlatformPresentationPrototype presentationPrototype, VehiclePrototype vehiclePrototype, CarryingPrototype carryingPrototype, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype) {
        this.position = position;
        this.presentationPrototype = presentationPrototype;
        this.vehiclePrototype = vehiclePrototype;
        this.carryingPrototype = carryingPrototype;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
    }
}
