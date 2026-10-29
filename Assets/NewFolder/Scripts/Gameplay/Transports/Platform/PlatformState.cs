
using UnityEngine;

public struct PlatformState {
    
    public Vector3 position;
    public bool hasLoadout;
    public LoadoutState loadoutState;
    public VehicleId vehicleId;
    public int platformId;

    public PlatformState(Vector3 position, LoadoutState loadoutState, VehicleId vehicleId, int platformId, bool hasLoadout) {
        this.position = position;
        this.loadoutState = loadoutState;
        this.vehicleId = vehicleId;
        this.platformId = platformId;
        this.hasLoadout = hasLoadout;
    }
}
