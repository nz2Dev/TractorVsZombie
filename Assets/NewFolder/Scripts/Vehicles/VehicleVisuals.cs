using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleVisuals : MonoBehaviour {

    [Serializable]
    public struct WheelAxis {
        public GameObject leftWheel;
        public GameObject rightWheel;
    }

    [SerializeField] private WheelAxis frontAxis;
    [SerializeField] private WheelAxis rearAxis;
    [SerializeField] private GameObject shaftGeometry;

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

    public void SetShaftRotation(Quaternion shaftRotation) {
        shaftGeometry.transform.rotation = shaftRotation;
    }

}
