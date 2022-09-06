using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class FollowZoomCameraZoomController : MonoBehaviour, ICameraZoomController, ICameraController {

    [SerializeField] private InputActionReference zoomActionRef;
    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private float initialZoomWidth = 15f;
    [SerializeField] private float widthChangeRange = 10f;
    [SerializeField] private float widthPerMember = 1f;

    private CinemachineFollowZoom _followZoom;
    private float _zoomWidthChangeLevel = 0;
    private bool _active;

    public event Action<float> OnZoomLevelChanged;

    private void Awake() {
        _followZoom = GetComponent<CinemachineFollowZoom>();
        caravanObserver.OnMembersChanged += OnCaravanChanged;
        zoomActionRef.action.performed += (ctx) => {
            if (_active) {
                SetZoomLevelInternal(_zoomWidthChangeLevel + ctx.ReadValue<float>());
                OnZoomLevelChanged?.Invoke(_zoomWidthChangeLevel);
            }
        };
    }

    private void OnCaravanChanged(CaravanObserver observer) {
        UpdateZoomWidths();
    }

    float ICameraZoomController.GetZoomLevel() {
        return _zoomWidthChangeLevel;
    }

    void ICameraZoomController.SetZoomLevel(float levelNoramlized) {
        SetZoomLevelInternal(levelNoramlized);
    }

    void ICameraController.OnActiveStateChanged(bool activeState) {
        _active = activeState;
    }

    private void SetZoomLevelInternal(float levelNoramlized) {
        _zoomWidthChangeLevel = Mathf.Clamp(levelNoramlized, -1, 1);
        UpdateZoomWidths();
    }

    [ContextMenu("Update Zoom Widths")]
    private void UpdateZoomWidths() {
        _followZoom.m_Width = initialZoomWidth - _zoomWidthChangeLevel * widthChangeRange + caravanObserver.CountedLength * widthPerMember;
    }
}