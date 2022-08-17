using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public interface ICameraZoomController {
    float GetZoomLevel();
    void SetZoomLevel(float levelNoramlized);
}

public interface ICameraOrbitController {
    void OrbitDelta(float horizontalDegree, float verticalDegree);
}

public class CamerasManager : MonoBehaviour {

    [SerializeField] private GroundObservable groundObservable;
    [SerializeField] private CinemachineVirtualCamera drivingCamera;
    [SerializeField] private CinemachineVirtualCamera holdingCamera; 
    [SerializeField] private float orbitSpeedMultiplier = 5;
    [SerializeField] private bool orbitOnGroundEvent;
    [SerializeField] private bool useDriving;

    public event Action<float> OnSelectedCameraZoomLevelChanged;

    private void Awake() {
        groundObservable.OnEvent += OnGroundEvent;
    }

    private void OnValidate() {
        UpdateCameraPriority();
    }

    private void Update() {
        UpdateCameraPriority();
    }

    private void OnGroundEvent(GroundObservable.EventType eventType, PointerEventData pointerEventData) {
        if (!orbitOnGroundEvent) {
            return;
        }

        var orbitXDelta = pointerEventData.delta.x / pointerEventData.pressEventCamera.pixelWidth;
        var orbitYDelta = pointerEventData.delta.y / pointerEventData.pressEventCamera.pixelHeight;

        var activeCamera = HigherPriorityCamera();
        var orbitController = activeCamera.GetComponent<ICameraOrbitController>();
        if (orbitController != null) {
            orbitController.OrbitDelta(
                orbitXDelta * Mathf.PI * Mathf.Rad2Deg * orbitSpeedMultiplier,
                orbitYDelta * Mathf.PI * Mathf.Rad2Deg * orbitSpeedMultiplier);
        }
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

        var caravanCamera = selectedCamera.GetComponent<ICameraZoomController>();
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
            var caravanCamera = higherAfter.GetComponent<ICameraZoomController>();
            OnSelectedCameraZoomLevelChanged?.Invoke(caravanCamera == null ? 0 : caravanCamera.GetZoomLevel());
        }
    }

    private CinemachineVirtualCamera HigherPriorityCamera() {
        return drivingCamera.Priority > holdingCamera.Priority ? drivingCamera : holdingCamera;
    }
}
