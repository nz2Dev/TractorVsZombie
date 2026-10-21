using UnityEngine;

internal sealed class VehicleModel {
    
    public VehicleId Id { get; }
    
    public PoseId HostPoseId { get; }
    public int VehiclePhysicsId { get; }
    
    public VehicleState State { get; set; }
    public float Gas { get; set; }
    public float Brakes { get; set; }
    public float Steer { get; set; }
    public bool SteerIsAngle { get; set; }

    public VehicleModel(VehicleId id, int vehiclePhysicsId, PoseId poseId, VehicleState state) {
        Id = id;
        VehiclePhysicsId = vehiclePhysicsId;
        HostPoseId = poseId;
        State = state;
    }
}
