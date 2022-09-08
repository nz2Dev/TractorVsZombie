using System;
using Cinemachine;
using UnityEngine;

public interface ICameraZoom {
    float GetZoomLevel();
    void SetZoomLevel(float levelNoramlized);
    void SetZoomFactor(int factor);
}

public interface ICameraRig {
    public Camera OutputCamera { get; }
    void Orbit(float horizontalDegree, float verticalDegree);
}

public interface IStatefulCameraRig : ICameraRig {
    void OnStartOrbiting() {
    }
    void OnEndOrbiting() {
    }
}

public interface ICameraStateListener {
    void OnActiveStateChanged(bool activeState);
}

public class CamerasManager : MonoBehaviour {

    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera drivingCamera;
    [SerializeField] private CinemachineVirtualCamera overviewCamera;
    [SerializeField] private CinemachineVirtualCamera topDownCamera;
    [SerializeField] private bool useDrivingInitialiy;

    public event Action<GameObject, GameObject> OnVCamChanged;

    public GameObject ActiveCam => brain.ActiveVirtualCamera.VirtualCameraGameObject;

    private void Start() {
        brain.m_CameraActivatedEvent.AddListener(OnActiveCameraChanged);
        OnActiveCameraChanged(brain.ActiveVirtualCamera, null);
    }

    private void OnActiveCameraChanged(ICinemachineCamera incomingCam, ICinemachineCamera outcomingCam) {
        var incomingListeners = incomingCam.VirtualCameraGameObject.GetComponents<ICameraStateListener>();
        if (incomingListeners != null) {
            foreach (var listener in incomingListeners) {
                listener.OnActiveStateChanged(activeState: true);
            }
        }

        if (outcomingCam != null) {
            var outcomingListeners = outcomingCam.VirtualCameraGameObject.GetComponents<ICameraStateListener>();
            if (outcomingListeners != null) {
                foreach (var listener in outcomingListeners) {
                    listener.OnActiveStateChanged(activeState: false);
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
