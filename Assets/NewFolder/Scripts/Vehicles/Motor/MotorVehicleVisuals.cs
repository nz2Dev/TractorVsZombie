using System;

using UnityEngine;

public class MotorVehicleVisuals : MonoBehaviour {
    
    [Serializable]
    public struct WheelAxis {
        public GameObject leftWheel;
        public GameObject rightWheel;
    }

    [SerializeField] private WheelAxis frontAxis;
    [SerializeField] private WheelAxis rearAxis;

    public void DestroySelf() {
        GameObject.Destroy(gameObject);
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot) {
        transform.SetPositionAndRotation(pos, rot);
    }

    public void SetFrontAxis(WheelAxisPose axisPose) {
        frontAxis.leftWheel.transform.SetPositionAndRotation(axisPose.positionL, axisPose.rotationL);
        frontAxis.rightWheel.transform.SetPositionAndRotation(axisPose.positionR, axisPose.rotationR);
    }
    
    public void SetRearAxis(WheelAxisPose axisPose) {
        rearAxis.leftWheel.transform.SetPositionAndRotation(axisPose.positionL, axisPose.rotationL);
        rearAxis.rightWheel.transform.SetPositionAndRotation(axisPose.positionR, axisPose.rotationR);
    }

}