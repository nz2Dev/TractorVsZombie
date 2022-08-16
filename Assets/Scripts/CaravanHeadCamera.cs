using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CaravanHeadCamera : MonoBehaviour {
    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private float memberZoomWidth = 0.5f;
    [SerializeField] private float minHeadZoomWidth = 5;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFollowZoom _followZoomExtension;

    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _followZoomExtension = GetComponent<CinemachineFollowZoom>();
        
        caravanObserver.OnMembersChanged += OnCaravanChanged;

        BindCameraTargets();
        UpdateFollowZoom();
    }

    private void OnCaravanChanged(CaravanObserver observer) {
        UpdateFollowZoom();
    }
    
    [ContextMenu("Bind Camera Targets")]
    private void BindCameraTargets() {
        _virtualCamera.Follow = caravanObserver.Subject.transform;
        _virtualCamera.LookAt = caravanObserver.Subject.transform;
    }

    private void UpdateFollowZoom() {
        _followZoomExtension.m_Width = minHeadZoomWidth + caravanObserver.CountedLength * memberZoomWidth;
    }
}
