using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class CaravanHeadVCamTargetAssigner : MonoBehaviour {
    [SerializeField] private CaravanObservable caravanObservable;
    
    private CinemachineVirtualCamera _virtualCamera;
    
    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        BindCameraTargets();
    }

    [ContextMenu("Bind Camera Targets")]
    private void BindCameraTargets() {
        _virtualCamera.Follow = caravanObservable.Subject.transform;
        _virtualCamera.LookAt = caravanObservable.Subject.transform;
    }
}
