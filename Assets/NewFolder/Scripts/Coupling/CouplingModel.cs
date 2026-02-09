using System.Collections.Generic;

public class CouplingModel {
    public int PullingVehiclePhysicsId { get; set; }
    public List<TowableVehicleId> TowableVehicleIds { get; private set; } = new ();
}