using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class OrbitalTransposerCameraOrbitController : MonoBehaviour, ICameraController {

    [SerializeField] private GroundObservable groundObservable;
    [SerializeField] private float orbitDistance = 15f;
    [SerializeField] private float orbitSpeedMultiplier = 1f;
    [SerializeField] private float initialOrbitVertically = 45;
    [SerializeField] private bool inverseVerticalOrbit = true;

    private CinemachineVirtualCamera _virtualCamera;
    private float _verticalOrbit;
    private bool _orbiting;
    private Camera _startOrbitingInitCamera;

    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        ResetOrbitRotation();        
    }

    public void OnActiveStateChanged(bool activeState) {
        if (activeState) {
            groundObservable.OnEvent += OnGroundEvent;
        } else {
            groundObservable.OnEvent -= OnGroundEvent;
        }
    }

    private void ResetOrbitRotation() {
        _verticalOrbit = initialOrbitVertically;
        UpdateOrbitOffset();
    }

    private void OnGroundEvent(GroundObservable.EventType eventType, PointerEventData pointerEventData) {
        if (eventType == GroundObservable.EventType.PointerDown && pointerEventData.button == 0) {
            _startOrbitingInitCamera = pointerEventData.pressEventCamera;
            StartOrbiting();
        }

        if (eventType == GroundObservable.EventType.PointerUp && pointerEventData.button == 0) {
            StopOrbiting();
        }
    }

    public void StartOrbiting() {
        Cursor.lockState = CursorLockMode.Locked;
        _orbiting = true;
        
        // recentering disabled
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        var recenter = orbitalTransposer.m_RecenterToTargetHeading;
        recenter.m_enabled = false;
        orbitalTransposer.m_RecenterToTargetHeading = recenter;
    }

    private void Update() {
        if (!_orbiting) {
            return;
        }

        var horizontalInput = Input.GetAxis("Mouse X") / _startOrbitingInitCamera.pixelWidth;
        var verticalInput = Input.GetAxis("Mouse Y") / _startOrbitingInitCamera.pixelHeight;

        var horizontalDegree = horizontalInput * Mathf.PI * Mathf.Rad2Deg * orbitSpeedMultiplier;
        var verticalDegree = verticalInput * Mathf.PI * Mathf.Rad2Deg * orbitSpeedMultiplier;
        // Debug.Log($"Orbiting {horizontalDegree}, {verticalDegree}");

        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        var xAxis = orbitalTransposer.m_XAxis;
        xAxis.Value += horizontalDegree;
        orbitalTransposer.m_XAxis = xAxis;

        _verticalOrbit += inverseVerticalOrbit ? -verticalDegree : verticalDegree;
        UpdateOrbitOffset();
    }

    public void StopOrbiting() {
        Cursor.lockState = CursorLockMode.None;
        _orbiting = false;

        //recentering enabled
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        var recenter = orbitalTransposer.m_RecenterToTargetHeading;
        recenter.m_enabled = true;
        orbitalTransposer.m_RecenterToTargetHeading = recenter;
    }

    [ContextMenu("Update Orbit Rotation")]
    private void UpdateOrbitOffset() {
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        
        var offset = orbitalTransposer.m_FollowOffset;
        offset.y = Mathf.Sin(Mathf.Deg2Rad * _verticalOrbit) * orbitDistance;
        offset.z = -Mathf.Cos(Mathf.Deg2Rad * _verticalOrbit) * orbitDistance;
        orbitalTransposer.m_FollowOffset = offset;

        // var heading = orbitalTransposer.m_Heading;
        // heading.m_Bias = _horizontalOrbit;
        // orbitalTransposer.m_Heading = heading;

        // var xAxis = orbitalTransposer.m_XAxis;
        // xAxis.Value = _horizontalOrbit;
        // orbitalTransposer.m_XAxis = xAxis;
    }
}