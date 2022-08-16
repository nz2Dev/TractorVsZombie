using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CamerasManager : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera drivingCamera;
    [SerializeField] private CinemachineVirtualCamera holdingCamera; 
    [SerializeField] private bool useDriving;

    private void OnValidate() {
        UpdateCameraPriority();
    }

    private void Update() {
        UpdateCameraPriority();
    }

    private void UpdateCameraPriority() {
        drivingCamera.Priority = useDriving ? 11 : 9;
        holdingCamera.Priority = useDriving ? 9 : 11;
    }
}
