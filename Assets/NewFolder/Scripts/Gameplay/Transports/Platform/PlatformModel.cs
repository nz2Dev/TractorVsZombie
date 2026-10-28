
public class PlatformModel {

    public PlatformModel(int id) {
        Id = id;
    }

    public int Id { get; }
    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public PoseId PoseId { get; set; }
    public VehicleId VehicleId { get; set; }
    public HitboxId HitboxId { get; set; }
    public CarryingId CarryingId { get; set; }

}
