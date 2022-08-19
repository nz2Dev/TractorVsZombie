using System;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class FollowZoomCameraZoomController : MonoBehaviour, ICameraZoomController {

    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private float initialZoomWidth = 15f;
    [SerializeField] private float widthChangeRange = 10f;
    [SerializeField] private float widthPerMember = 1f;

    private CinemachineFollowZoom _followZoom;
    private float _zoomWidthChangeLevel = 0;

    private void Awake() {
        _followZoom = GetComponent<CinemachineFollowZoom>();
        caravanObserver.OnMembersChanged += OnCaravanChanged;
    }

    private void OnCaravanChanged(CaravanObserver observer) {
        UpdateZoomWidths();
    }

    float ICameraZoomController.GetZoomLevel() {
        return _zoomWidthChangeLevel;
    }

    void ICameraZoomController.SetZoomLevel(float levelNoramlized) {
        _zoomWidthChangeLevel = levelNoramlized;
        UpdateZoomWidths();
    }

    [ContextMenu("Update Zoom Widths")]
    private void UpdateZoomWidths() {
        _followZoom.m_Width = initialZoomWidth + _zoomWidthChangeLevel * widthChangeRange + caravanObserver.CountedLength * widthPerMember;
    }
}