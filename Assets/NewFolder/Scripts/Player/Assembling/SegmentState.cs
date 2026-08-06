using UnityEngine;

public class SegmentState {
    public bool isTruck;
    public int platformId = -1;
    public bool waitsActivation;
    public bool isConnected;
    public Vector3 activationPosition;
    public LoadoutPrototype activationLoadout;

    public bool IsPlatformCreated => !isTruck && platformId != -1;
}
