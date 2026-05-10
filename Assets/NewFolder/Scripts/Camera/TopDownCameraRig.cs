using System;

using Cinemachine;

using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class TopDownCameraRig : MonoBehaviour {

    [SerializeField] private float distance = 28;
    [SerializeField] private Transform targetTransform;
    
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;

#if UNITY_EDITOR
    private void OnValidate() {
        var virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.m_Follow = targetTransform;
        virtualCamera.m_LookAt = targetTransform;  
    }
#endif

    private void Awake() {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        
        var cameraManager = GameObject.FindFirstObjectByType<CameraManager>();
        cameraManager.SetTopDownCameraRig(this);
    }

    private void Update() {
        framingTransposer.m_CameraDistance = distance;
    }

    public void UpdateFollowPosition(Vector3 followPosition) {
        targetTransform.position = followPosition;
    }

}