using UnityEngine;

public class Vehicle {
    
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public WheelAxisPose[] WheelAxes { get; private set; }
    public TowingWheelAxisPose[] TowingWheelAxes { get; private set; }

}