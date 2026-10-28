
using UnityEngine;

public struct PlatformState {
    
    public Vector3 position;
    public HealthId healthId;
    public HealthState healthState;
    public FactionState factionState;
    public int weaponId;
    public WeaponState weaponState;
    public VehicleId vehicleId;
    public int platformId;

    public PlatformState(Vector3 position, HealthId healthId, HealthState healthState, FactionState factionState, int weaponId, WeaponState weaponState, VehicleId vehicleId, int platformId) {
        this.position = position;
        this.healthId = healthId;
        this.healthState = healthState;
        this.factionState = factionState;
        this.weaponId = weaponId;
        this.weaponState = weaponState;
        this.vehicleId = vehicleId;
        this.platformId = platformId;
    }
}
