using System.Collections.Generic;

using UnityEngine;

public struct DrivingInput {
    public float gas;
    public float steering;
    public bool boost;
}

public struct TopDownAimInput {
    public float height;
    public Vector3 position; 
    public Vector3 direction;
}

public class PlayerModel {

    public PlayerConfig Config { get; }

    public TruckPrototype TruckPrototype { get; set; }
    public PlatformPrototype PickupPlatformPrototype { get; set; }
    
    public Vector3 Position { get; set; }
    public DrivingInput DrivingInput { get; set; }
    public List<int> SelectedPlatformIds { get; set; } = new ();
    public List<int> CoupledPlatformIds { get; } = new ();
    public List<int> ControlledPlatformIds { get; } = new ();
    public TopDownAimInput AimInput { get; set; }

    public PlayerModel(PlayerConfig config) {
        this.Config = config;
    }

}