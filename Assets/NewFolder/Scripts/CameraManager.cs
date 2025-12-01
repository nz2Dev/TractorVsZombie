using Cinemachine;

using UnityEngine;

public class CameraManager : MonoBehaviour {

    [SerializeField] private Camera sceneCamera;
    [SerializeField] private float topDownCameraDistance = 10;
    [SerializeField] private CinemachineVirtualCamera topDownCamera;

    private Transform followTransform;

    private void Awake() {
        if (sceneCamera.GetComponent<CinemachineBrain>() == null) {
            sceneCamera.gameObject.AddComponent<CinemachineBrain>();
        }
        
        var followGameObject = new GameObject("Camera Follow Target (New)");
        followTransform = followGameObject.transform;
        topDownCamera.m_Follow = followTransform;
        topDownCamera.m_LookAt = followTransform;
    }

    public Ray GetCameraRay(Vector2 mousePositionInPixels) {
        return sceneCamera.ScreenPointToRay(mousePositionInPixels);
    }

    private void Update() {
        var framingTransposer = topDownCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_CameraDistance = topDownCameraDistance;
    }

    public void InitTopDownFollowTarget(Vector3 initPosition) {
        followTransform.position = initPosition;
        CinemachineBrain.SoloCamera = topDownCamera;
    }

    public void UpdateTopDownFollowPosition(Vector3 vector3) {
        followTransform.position = vector3;
    }
}