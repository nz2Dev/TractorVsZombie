using System.Collections.Generic;

public class AssemblingModel {
    public TruckPrototype TruckPrototype { get; set; }
    public PlatformPrototype PickupPlatformPrototype { get; set; }
    public List<int> ControlledPlatformIds { get; } = new ();
    public List<int> CoupledPlatformIds { get; } = new ();
}