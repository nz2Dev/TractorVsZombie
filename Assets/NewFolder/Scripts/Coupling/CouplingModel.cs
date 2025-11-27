using System.Collections.Generic;

public class CouplingModel {
    public MotorVehicleId PullingVehicleId { get; set; }
    public List<TowableVehicleId> TowableVehicleIds { get; private set; } = new ();
}