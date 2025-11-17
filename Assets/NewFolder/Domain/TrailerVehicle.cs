using UnityEngine;

public class TrailerVehicle {
    
    private readonly TrailerVehicleData data;
    
    private VehicleState vehicleState;
    public Quaternion TowingTonqueRotation { get; private set; }

    public TrailerVehicle(TrailerVehicleData data) {
        this.data = data;
    }

    public Vector3 Position => vehicleState.position;
    public VehicleState PhysicsState => vehicleState;
    public VehiclePhysics PhysicsPrefab => data.physicsPrefab;
    public VehicleVisuals VisualsData => data.visualsPrefab;

    public void UpdatePhysicsState(VehicleState state) {
        vehicleState = state;
    }

    public void OrientTowingTonque(Quaternion rotation) {
        TowingTonqueRotation = rotation;
    }
}