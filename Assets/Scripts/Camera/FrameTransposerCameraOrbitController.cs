using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class FrameTransposerCameraOrbitController : MonoBehaviour, ICameraController {

    [SerializeField] private GroundObservable groundObservable;
    [SerializeField] private float orbitSpeedMultiplier = 1;
    [SerializeField] private bool inverseVerticalOrbit = true;
    [SerializeField][Range(-180, 180)] private float initialOrbitHorizontally = 0;
    [SerializeField][Range(0, 89)] private float initialOrbitVertically = 75f;

    private float _horizontalOrbit;
    private float _verticalOrbit;
    private bool _orbiting;

    private void Awake() {
        ResetOrbitRotation();
    }

    public void OnActiveStateChanged(bool activeState) {
        if (activeState) {
            groundObservable.OnEvent += OnGroundEvent;
        } else {
            groundObservable.OnEvent -= OnGroundEvent;
        }
    }

    private void OnGroundEvent(GroundObservable.EventType eventType, PointerEventData pointerEventData) {
        if (eventType == GroundObservable.EventType.PointerDown && pointerEventData.button == 0) {
            _orbiting = true;
        }
        if (eventType == GroundObservable.EventType.PointerUp && pointerEventData.button == 0) {
            _orbiting = false;
        }

        if (!_orbiting) {
            return;
        }
        
        var orbitXDelta = pointerEventData.delta.x / pointerEventData.pressEventCamera.pixelWidth;
        var orbitYDelta = pointerEventData.delta.y / pointerEventData.pressEventCamera.pixelHeight;

        OrbitCamera(
            orbitXDelta * Mathf.PI * Mathf.Rad2Deg * orbitSpeedMultiplier,
            orbitYDelta * Mathf.PI * Mathf.Rad2Deg * orbitSpeedMultiplier);
    }

    private void OrbitCamera(float horizontalDegree, float verticalDegree) {
        _horizontalOrbit += horizontalDegree;
        _verticalOrbit += inverseVerticalOrbit ? -verticalDegree : verticalDegree;
        UpdateOrbitRotation();
    }

    [ContextMenu("Reset Orbit Rotation")]
    private void ResetOrbitRotation() {
        _horizontalOrbit = initialOrbitHorizontally;
        _verticalOrbit = initialOrbitVertically;
        UpdateOrbitRotation();
    }

    [ContextMenu("Update Orbit Rotation")]
    private void UpdateOrbitRotation() {
        transform.rotation = Quaternion.Euler(_verticalOrbit, _horizontalOrbit, 0);
    }
}