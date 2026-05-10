using Cinemachine;

using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CinemachineBrain))]
public class CinemachineSource : MonoBehaviour {

    public Camera cameraRef;
    public CinemachineBrain cinemachineBrain;

    private void Awake() {
        cameraRef = GetComponent<Camera>();
        cinemachineBrain = GetComponent<CinemachineBrain>();

        var cameraManager = FindFirstObjectByType<CameraManager>();
        if (cameraManager != null) {
            cameraManager.SetCinemachineSource(this);
        }
    }
}