

using UnityEngine;

public class ArmorModel {

    public ArmorModel(int id) {
        Id = id;
    }

    public int Id { get; }
    
    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public VehicleId VehicleId { get; set; }
    public PoseId PoseId { get; set; }
    public HitboxId HitboxId { get; set; }
    public DropId DropId { get; set; }
    public AttachmentId AttachmentId { get; set; }
    public RamId RamId { get; set; }

    public HealthState HealthState { get; set; }
}
