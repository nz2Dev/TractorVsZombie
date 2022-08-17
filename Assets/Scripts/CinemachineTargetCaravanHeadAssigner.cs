using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class CinemachineTargetCaravanHeadAssigner : MonoBehaviour {
    [SerializeField] private CaravanObserver caravanObserver;
    
    private CinemachineVirtualCamera _virtualCamera;
    
    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        BindCameraTargets();
    }

    [ContextMenu("Bind Camera Targets")]
    private void BindCameraTargets() {
        _virtualCamera.Follow = caravanObserver.Subject.transform;
        _virtualCamera.LookAt = caravanObserver.Subject.transform;
    }
}
