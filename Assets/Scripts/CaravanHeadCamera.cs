using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CaravanHeadCamera : MonoBehaviour, ICaravanCamera {
    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private float memberZoomWidth = 0.5f;
    [SerializeField] private float minHeadZoomWidth = 5;
    [SerializeField] private float zoomWidthChangeRange = 10f;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFollowZoom _followZoomExtension;
    private float _zoomWidthRangeLevel = 0;

    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _followZoomExtension = GetComponent<CinemachineFollowZoom>();
        caravanObserver.OnMembersChanged += OnCaravanChanged;
    }

    private void Start() {
        BindCameraTargets();
        UpdateFollowZoom();
    }

    private void OnCaravanChanged(CaravanObserver observer) {
        UpdateFollowZoom();
    }
    
    public void SetZoomLevel(float levelNoramlized) {
        _zoomWidthRangeLevel = Mathf.Clamp01(levelNoramlized);

        if (_followZoomExtension != null) {
            UpdateFollowZoom();
        }
    }
    
    [ContextMenu("Bind Camera Targets")]
    private void BindCameraTargets() {
        _virtualCamera.Follow = caravanObserver.Subject.transform;
        _virtualCamera.LookAt = caravanObserver.Subject.transform;
    }

    private void UpdateFollowZoom() {
        _followZoomExtension.m_Width = minHeadZoomWidth + zoomWidthChangeRange * _zoomWidthRangeLevel + caravanObserver.CountedLength * memberZoomWidth;
    }
}
