using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class FollowWidthCameraZoom : MonoBehaviour, ICameraZoom {

    [SerializeField] private float initialZoomWidth = 15f;
    [SerializeField] private float widthChangeRange = 10f;
    [SerializeField] private float widthPerFactor = 1f;

    private CinemachineFollowZoom _followZoom;
    private int _zoomWidthChangeFactor = 0;
    private float _zoomWidthChangeLevel = 0;

    private void Awake() {
        _followZoom = GetComponent<CinemachineFollowZoom>();
    }

    private void OnCaravanChanged(CaravanObservable observer) {
        UpdateZoomWidths();
    }

    void ICameraZoom.SetZoomFactor(int factor) {
        _zoomWidthChangeFactor = factor;
    }

    float ICameraZoom.GetZoomLevel() {
        return _zoomWidthChangeLevel;
    }

    void ICameraZoom.SetZoomLevel(float levelNoramlized) {
        SetZoomLevelInternal(levelNoramlized);
    }

    private void SetZoomLevelInternal(float levelNoramlized) {
        _zoomWidthChangeLevel = Mathf.Clamp(levelNoramlized, -1, 1);
        UpdateZoomWidths();
    }

    [ContextMenu("Update Zoom Widths")]
    private void UpdateZoomWidths() {
        _followZoom.m_Width = initialZoomWidth - _zoomWidthChangeLevel * widthChangeRange + _zoomWidthChangeFactor * widthPerFactor;
    }
}