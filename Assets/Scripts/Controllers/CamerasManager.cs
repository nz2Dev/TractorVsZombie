using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public interface ICameraZoomController {
    event Action<float> OnZoomLevelChanged;
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
    public event Action<GameObject, GameObject> OnVCamChanged;

    public GameObject ActiveCam => HigherPriorityCamera().VirtualCameraGameObject;

    private void Start() {
        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(drivingCamera);
        brain.m_CameraActivatedEvent.AddListener(OnActiveCameraChanged);
        OnActiveCameraChanged(HigherPriorityCamera(), null);
    }

    private void OnActiveCameraChanged(ICinemachineCamera incomingCam, ICinemachineCamera outcomingCam) {
        var incomingControllers = incomingCam.VirtualCameraGameObject.GetComponents<ICameraController>();
        if (incomingControllers != null) {
            foreach (var controller in incomingControllers) {
                controller.OnActiveStateChanged(activeState: true);
            }
        }

        if (outcomingCam != null) {
            var outcomingControllers = outcomingCam.VirtualCameraGameObject.GetComponents<ICameraController>();
            if (outcomingControllers != null) {
                foreach (var controller in outcomingControllers) {
                    controller.OnActiveStateChanged(activeState: false);
                }
            }
        }

        // Debug.Log($"{incomingCam.VirtualCameraGameObject.name} enabled {outcomingCam?.VirtualCameraGameObject?.name ?? "null"} disabled");
        OnVCamChanged?.Invoke(outcomingCam == null ? null : outcomingCam.VirtualCameraGameObject, incomingCam.VirtualCameraGameObject);
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
