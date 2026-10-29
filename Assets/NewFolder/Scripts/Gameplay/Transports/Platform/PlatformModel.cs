
public class PlatformModel {

    public PlatformModel(int id) {
        Id = id;
    }

    public int Id { get; }
    public FactionId FactionId { get; set; }
    public PoseId PoseId { get; set; }
    public VehicleId VehicleId { get; set; }
    public CarryingId CarryingId { get; set; }

}
