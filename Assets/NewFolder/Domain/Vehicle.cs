using UnityEngine;

public class Vehicle {
    
    public int Id { get; set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public WheelAxisPose[] WheelAxisPoses { get; private set; }
    public TowingWheelAxisPose? TowingWheelAxisPose { get; private set; }

    public void Configure(int wheelAxisCount, bool hasTowingWheelAxis) {
        WheelAxisPoses = new WheelAxisPose[wheelAxisCount];
        TowingWheelAxisPose = hasTowingWheelAxis ? default(TowingWheelAxisPose) : null;
    }

    public void Orient(Vector3 position, Quaternion rotation) {
        Position = position;
        Rotation = rotation;
    }

    public void OrientWheelAxis(int index, WheelAxisPose pose) {
        WheelAxisPoses[index] = pose;
    }

    public void OrientTowingWheelAxis(TowingWheelAxisPose pose) {
        TowingWheelAxisPose = pose;
    }

}