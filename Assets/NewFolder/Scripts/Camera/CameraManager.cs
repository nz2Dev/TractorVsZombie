using Cinemachine;

using UnityEngine;

// Is presentation layer subsystem that is the main authority over what camera rig is currently used
// and how camera rig behave, and what camera it operate
// It's isolaed from gameplay and its controllers via Engine/CameraProvider.cs
// ***
// The name: Camera, indicates that this relates to Presentation/Application, not to a gameplay feature
// I keep it in separate Folder, just to break folder coupling with GameBootstrapper
// **
// It doesn't fit inside Local/ "packages" semantic as it's not a library in its sense, it's tight to the application we develop
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
        
        // Ensure we start with a standard priority
        topDownCamera.Priority = 10;
    }

    public Camera GetActiveCamera() {
        return sceneCamera;
    }

    public CinemachineBrain GetBrain() {
        return sceneCamera.GetComponent<CinemachineBrain>();
    }

    private void Update() {
        var framingTransposer = topDownCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (framingTransposer != null) {
            framingTransposer.m_CameraDistance = topDownCameraDistance;
        }
    }

    public void SetCutsceneState(bool isCutscene) {
        // When in cutscene, we lower the gameplay camera priority so other cameras (like Timeline shots) take over.
        // When cutscene ends, we raise it back to trigger a smooth blend.
        // topDownCamera.Priority = isCutscene ? 0 : 20;
        
        // Important: SoloCamera overrides all blending logic. 
        // We clear it to ensure Cinemachine uses its standard priority-based blending.
        // CinemachineBrain.SoloCamera = null;
    }

    public void InitTopDownFollowTarget(Vector3 initPosition) {
        followTransform.position = initPosition;
        SetCutsceneState(false);
    }

    public void UpdateTopDownFollowPosition(Vector3 vector3) {
        followTransform.position = vector3;
    }
}