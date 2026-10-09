using System;

using Cinemachine;

using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class TopDownCameraRig : MonoBehaviour {

    [SerializeField] private float distance = 28;
    [SerializeField] private Transform targetTransform;
    
    private CinemachineFramingTransposer framingTransposer;
    
    internal CinemachineVirtualCamera VirtualCamera { get; private set; }

#if UNITY_EDITOR
    private void OnValidate() {
        var virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.m_Follow = targetTransform;
        virtualCamera.m_LookAt = targetTransform;  
    }
#endif

    private void Awake() {
        VirtualCamera = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update() {
        framingTransposer.m_CameraDistance = distance;
    }

    public void UpdateFollowPosition(Vector3 followPosition) {
        targetTransform.position = followPosition;
    }

}