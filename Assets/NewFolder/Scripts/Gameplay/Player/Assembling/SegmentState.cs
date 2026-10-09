using UnityEngine;

public class SegmentState {
    public bool isTruck;
    public int platformId = -1;
    public bool waitsActivation;
    public int connectedVehiclePhysicsId = -1;
    public Vector3 activationPosition;
    public LoadoutPrototype activationLoadout;

    public bool IsPlatformCreated => !isTruck && platformId != -1;
    public bool IsPlatformConnected => connectedVehiclePhysicsId != -1;
    public void ResetConnectedPlatformId() {
        connectedVehiclePhysicsId = -1;
    }
}
