using System;

using UnityEngine;

public class VehicleWheelRig : MonoBehaviour {

    [Serializable]
    public struct WheelAxis {
        public GameObject leftWheel;
        public GameObject rightWheel;
    }

    [SerializeField] private WheelAxis frontAxis;
    [SerializeField] private WheelAxis rearAxis;

    public void SetFrontAxis(WheelAxisPose axisPose) {
        SetAxis(frontAxis, axisPose);
    }

    public void SetRearAxis(WheelAxisPose axisPose) {
        SetAxis(rearAxis, axisPose);
    }

    private static void SetAxis(WheelAxis wheels, WheelAxisPose axisPose) {
        wheels.leftWheel.transform.SetPositionAndRotation(axisPose.positionL, axisPose.rotationL);
        wheels.rightWheel.transform.SetPositionAndRotation(axisPose.positionR, axisPose.rotationR);
    }
}
