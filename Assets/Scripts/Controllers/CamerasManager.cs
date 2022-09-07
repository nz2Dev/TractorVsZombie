using System;
using Cinemachine;
using UnityEngine;

public interface ICameraZoomController {
    event Action<float> OnZoomLevelChanged;
    float GetZoomLevel();
    void SetZoomLevel(float levelNoramlized);
}

public interface ICameraRig {
    public string orbitActivationInputHint { get; }
    public string orbitPerformingInputHint { get; }
}

public interface ICameraController {
    void OnActiveStateChanged(bool activeState);
}

public class CamerasManager : MonoBehaviour {

    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera drivingCamera;
    [SerializeField] private CinemachineVirtualCamera overviewCamera;
    [SerializeField] private CinemachineVirtualCamera topDownCamera;
    [SerializeField] private bool useDrivingInitialiy;

    public event Action<float> OnSelectedCameraZoomLevelChanged;
    public event Action<GameObject, GameObject> OnVCamChanged;

    public GameObject ActiveCam => brain.ActiveVirtualCamera.VirtualCameraGameObject;

    private void Start() {
        brain.m_CameraActivatedEvent.AddListener(OnActiveCameraChanged);
        OnActiveCameraChanged(brain.ActiveVirtualCamera, null);
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

    public void SetDrivingCamera() {
        drivingCamera.Priority = 11;
        overviewCamera.Priority = 9;
        topDownCamera.Priority = 9;
    }

    public void SetOverviewCamera() {
        drivingCamera.Priority = 9;
        overviewCamera.Priority = 11;
        topDownCamera.Priority = 9;
    }

    public void SetTopDownCamera() {
        drivingCamera.Priority = 9;
        overviewCamera.Priority = 9;
        topDownCamera.Priority = 11;
    }
}
