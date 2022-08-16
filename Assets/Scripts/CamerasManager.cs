using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public interface ICaravanCamera {
    float GetZoomLevel();
    void SetZoomLevel(float levelNoramlized);
}

public class CamerasManager : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera drivingCamera;
    [SerializeField] private CinemachineVirtualCamera holdingCamera; 
    [SerializeField] private bool useDriving;

    public event Action<float> OnSelectedCameraZoomLevelChanged;

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
        if (caravanCamera != null) {
            caravanCamera.SetZoomLevel(zoomNormalized);
        }
    }

    private void UpdateCameraPriority() {
        if (drivingCamera == null || holdingCamera == null) {
            return;
        }

        var higherBefore = HigherPriorityCamera();

        drivingCamera.Priority = useDriving ? 11 : 9;
        holdingCamera.Priority = useDriving ? 9 : 11;

        var higherAfter = HigherPriorityCamera();
        if (higherAfter != higherBefore) {
            var caravanCamera = higherAfter.GetComponent<ICaravanCamera>();
            OnSelectedCameraZoomLevelChanged?.Invoke(caravanCamera == null ? 0 : caravanCamera.GetZoomLevel());
        }
    }

    private CinemachineVirtualCamera HigherPriorityCamera() {
        return drivingCamera.Priority > holdingCamera.Priority ? drivingCamera : holdingCamera;
    }
}
