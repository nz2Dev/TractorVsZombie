using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class OrbitalTransposerCameraRig : MonoBehaviour, ICameraStateListener, IStatefulCameraRig {

    [SerializeField] private float orbitDistance = 15f;
    [SerializeField] private float orbitSpeedMultiplier = 1f;
    [SerializeField] private float initialOrbitVertically = 45;
    [SerializeField] private bool inverseVerticalOrbit = true;

    private CinemachineVirtualCamera _virtualCamera;
    private Camera _startOrbitingInitCamera;
    private float _verticalOrbit;

    Camera ICameraRig.OutputCamera => _startOrbitingInitCamera;

    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        ResetOrbitRotation();
    }

    public void OnActiveStateChanged(bool activeState) {
        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(_virtualCamera);
        _startOrbitingInitCamera = brain == null ? null : brain.OutputCamera;
    }

    private void ResetOrbitRotation() {
        _verticalOrbit = initialOrbitVertically;
        UpdateOrbitOffset();
    }

    void IStatefulCameraRig.OnStartOrbiting() {
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        var recenter = orbitalTransposer.m_RecenterToTargetHeading;
        recenter.m_enabled = false;
        orbitalTransposer.m_RecenterToTargetHeading = recenter;
    }

    void ICameraRig.Orbit(float horizontalDegree, float verticalDegree) {
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        
        var xAxis = orbitalTransposer.m_XAxis;
        xAxis.Value += horizontalDegree * orbitSpeedMultiplier;
        orbitalTransposer.m_XAxis = xAxis;

        _verticalOrbit += (inverseVerticalOrbit ? -verticalDegree : verticalDegree) * orbitSpeedMultiplier;
        UpdateOrbitOffset();
    }

    void IStatefulCameraRig.OnEndOrbiting() {
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