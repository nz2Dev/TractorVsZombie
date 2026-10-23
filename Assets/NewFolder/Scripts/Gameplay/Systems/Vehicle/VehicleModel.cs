using UnityEngine;

internal sealed class VehicleModel {
    
    public VehicleId Id { get; }
    
    public PoseId HostPoseId { get; }
    public int VehiclePhysicsId { get; }
    public float BaseLinearDrag { get; }
    public float TemporalLinearDrag { get; set; }
    public VehicleConfig Config { get; }
    
    public float Gas { get; set; }
    public float Brakes { get; set; }
    public float Steer { get; set; }
    public bool SteerIsAngle { get; set; }

    public VehicleModel(VehicleId id, int vehiclePhysicsId, PoseId poseId, float baseLinearDrag, VehicleConfig config) {
        Id = id;
        VehiclePhysicsId = vehiclePhysicsId;
        HostPoseId = poseId;
        BaseLinearDrag = baseLinearDrag;
        Config = config;
    }
}
