using System.Collections.Generic;

using UnityEngine;

public class AimingModel {
    public TopDownAimInput AimInput { get; set; }
    public List<int> ManualPlatformIds { get; } = new ();
}
