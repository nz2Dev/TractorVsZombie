using System;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitalTransposerCameraOrbitController : MonoBehaviour, ICameraOrbitController {

    [SerializeField] private float orbitDistance = 15f;
    [SerializeField] private float initialOrbitVertically = 45;
    [SerializeField] private bool inverseVerticalOrbit = true;

    private float _verticalOrbit;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        ResetOrbitRotation();
    }

    private void ResetOrbitRotation() {
        _verticalOrbit = initialOrbitVertically;
        UpdateOrbitOffset();
    }

    public void StartOrbiting() {
        Cursor.lockState = CursorLockMode.Confined;
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        var recenter = orbitalTransposer.m_RecenterToTargetHeading;
        //recenter.m_enabled = false;
        orbitalTransposer.m_RecenterToTargetHeading = recenter;
    }

    public void OrbitDelta(float horizontalDegree, float verticalDegree) {
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        var xAxis = orbitalTransposer.m_XAxis;
        xAxis.Value += horizontalDegree;
        orbitalTransposer.m_XAxis = xAxis;

        _verticalOrbit += inverseVerticalOrbit ? -verticalDegree : verticalDegree;
        UpdateOrbitOffset();
    }

    public void StopOrbiting() {
        Cursor.lockState = CursorLockMode.None;

        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        var recenter = orbitalTransposer.m_RecenterToTargetHeading;
        //recenter.m_enabled = true;
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