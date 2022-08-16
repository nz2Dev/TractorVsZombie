using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public interface ICaravanCamera {
    void SetZoomLevel(float levelNoramlized);
}

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

    public void SetCameraType(bool driving) {
        useDriving = driving;
        UpdateCameraPriority();
    }

    public void SetCameraZoomLevel(float zoomNormalized) {
        var selectedCamera = HigherPriorityCamera();
        if (selectedCamera == null) {
            return;
        }

        var caravanCamera = selectedCamera.GetComponent<ICaravanCamera>();
        caravanCamera.SetZoomLevel(zoomNormalized);
    }

    private void UpdateCameraPriority() {
        if (drivingCamera == null || holdingCamera == null) {
            return;
        }

        drivingCamera.Priority = useDriving ? 11 : 9;
        holdingCamera.Priority = useDriving ? 9 : 11;
    }

    private CinemachineVirtualCamera HigherPriorityCamera() {
        return drivingCamera.Priority > holdingCamera.Priority ? drivingCamera : holdingCamera;
    }
}
