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

public interface ICameraController {
    void OnActiveStateChanged(bool activeState);
}

public class CamerasManager : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera drivingCamera;
    [SerializeField] private CinemachineVirtualCamera holdingCamera;
    [SerializeField] private bool useDriving;

    public event Action<float> OnSelectedCameraZoomLevelChanged;
    public event Action<GameObject> OnVCamChanged;

    private void Start() {
        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(drivingCamera);
        brain.m_CameraActivatedEvent.AddListener(OnActiveCameraChanged);
        OnActiveCameraChanged(HigherPriorityCamera(), null);
    }

    private void OnActiveCameraChanged(ICinemachineCamera incomingCam, ICinemachineCamera outcomingCam) {
        var incomingController = incomingCam.VirtualCameraGameObject.GetComponent<ICameraController>();
        if (incomingController != null) {
            incomingController.OnActiveStateChanged(activeState: true);
        }

        if (outcomingCam != null) {
            var outcomingController = outcomingCam.VirtualCameraGameObject.GetComponent<ICameraController>();
            if (outcomingController != null) {
                outcomingController.OnActiveStateChanged(activeState: false);
            }
        }

        // Debug.Log($"{incomingCam.VirtualCameraGameObject.name} enabled {outcomingCam?.VirtualCameraGameObject?.name ?? "null"} disabled");
        OnVCamChanged?.Invoke(incomingCam.VirtualCameraGameObject);
    }

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
