using UnityEngine;

public struct WheelAxisPose {
    public Vector3 positionL;
    public Quaternion rotationL;
    public Vector3 positionR;
    public Quaternion rotationR;
}

public struct TowingWheelAxisPose {
    public Vector3 positionL;
    public Quaternion rotationL;
    public Vector3 positionR;
    public Quaternion rotationR;
    public Quaternion tipRotation;
}

public struct VehiclePose {
    public Vector3 position;
    public Quaternion rotation;
}