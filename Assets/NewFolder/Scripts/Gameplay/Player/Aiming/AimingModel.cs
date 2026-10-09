using System.Collections.Generic;

using UnityEngine;

public class AimingModel {
    public Vector3 AimSourcePosition { get; set; }
    public TopDownAimInput AimInput { get; set; }
    public List<int> ManualPlatformIds { get; } = new ();
    public List<int> ControlledPlatformIds { get; } = new ();
}