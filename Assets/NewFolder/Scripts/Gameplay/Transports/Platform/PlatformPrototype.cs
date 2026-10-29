
using UnityEngine;

[System.Serializable]
public struct PlatformPrototype {
    public Vector3 position;
    public PlatformPresentationPrototype presentationPrototype;
    public VehiclePrototype vehiclePrototype;
    public CarryingPrototype carryingPrototype;
    public FactionPrototype factionPrototype;

    public PlatformPrototype(Vector3 position, PlatformPresentationPrototype presentationPrototype, VehiclePrototype vehiclePrototype, CarryingPrototype carryingPrototype, FactionPrototype factionPrototype) {
        this.position = position;
        this.presentationPrototype = presentationPrototype;
        this.vehiclePrototype = vehiclePrototype;
        this.carryingPrototype = carryingPrototype;
        this.factionPrototype = factionPrototype;
    }
}
