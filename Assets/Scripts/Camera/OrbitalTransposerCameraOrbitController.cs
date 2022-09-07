using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class OrbitalTransposerCameraOrbitController : MonoBehaviour, ICameraController, ICameraRig {

    [SerializeField] private InputActionReference cameraEngage;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float orbitDistance = 15f;
    [SerializeField] private float orbitSpeedMultiplier = 1f;
    [SerializeField] private float initialOrbitVertically = 45;
    [SerializeField] private bool inverseVerticalOrbit = true;

    private CinemachineVirtualCamera _virtualCamera;
    private Camera _startOrbitingInitCamera;
    private float _verticalOrbit;
    private bool _orbiting;
    private bool _active;

    string ICameraRig.orbitActivationInputHint => cameraEngage.action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions | InputBinding.DisplayStringOptions.DontOmitDevice);
    string ICameraRig.orbitPerformingInputHint => lookAction.action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontOmitDevice);

    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        ResetOrbitRotation();
    }

    public void OnActiveStateChanged(bool activeState) {
        _active = activeState;
        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(_virtualCamera);
        _startOrbitingInitCamera = brain == null ? null : brain.OutputCamera;
    }

    private void ResetOrbitRotation() {
        _verticalOrbit = initialOrbitVertically;
        UpdateOrbitOffset();
    }

    private void Update() {
        if (!_active)
            return;

        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        
        if (cameraEngage.action.inProgress && cameraEngage.action.WasPressedThisFrame()) {
            Cursor.lockState = CursorLockMode.Locked;
            var recenter = orbitalTransposer.m_RecenterToTargetHeading;
            recenter.m_enabled = false;
            orbitalTransposer.m_RecenterToTargetHeading = recenter;
        }

        if (cameraEngage.action.WasPerformedThisFrame() && cameraEngage.action.WasReleasedThisFrame()) {
            Cursor.lockState = CursorLockMode.None;
            var recenter = orbitalTransposer.m_RecenterToTargetHeading;
            recenter.m_enabled = true;
            orbitalTransposer.m_RecenterToTargetHeading = recenter;
        }
        
        _orbiting = cameraEngage.action.inProgress;
        if (!_orbiting)
            return;

        var lookInput = lookAction.action.ReadValue<Vector2>();
        var horizontalInput = lookInput.x / _startOrbitingInitCamera.pixelWidth;
        var verticalInput = lookInput.y / _startOrbitingInitCamera.pixelHeight;

        var horizontalDegree = horizontalInput * Mathf.PI * Mathf.Rad2Deg * orbitSpeedMultiplier;
        var verticalDegree = verticalInput * Mathf.PI * Mathf.Rad2Deg * orbitSpeedMultiplier;
        // Debug.Log($"Orbiting {horizontalDegree}, {verticalDegree}");

        var xAxis = orbitalTransposer.m_XAxis;
        xAxis.Value += horizontalDegree;
        orbitalTransposer.m_XAxis = xAxis;

        _verticalOrbit += inverseVerticalOrbit ? -verticalDegree : verticalDegree;
        UpdateOrbitOffset();
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