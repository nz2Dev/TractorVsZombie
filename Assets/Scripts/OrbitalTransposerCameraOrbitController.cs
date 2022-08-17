using System;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitalTransposerCameraOrbitController : MonoBehaviour, ICameraOrbitController {

    [SerializeField] private float orbitDistance = 15f;
    [SerializeField] private float initialOrbitHorizontally = 0;
    [SerializeField] private float initialOrbitVertically = 45;
    [SerializeField] private bool inverseVerticalOrbit = true;

    private float _horizontalOrbit;
    private float _verticalOrbit;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake() {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        ResetOrbitRotation();
    }

    private void ResetOrbitRotation() {
        _horizontalOrbit = initialOrbitHorizontally;
        _verticalOrbit = initialOrbitVertically;
        UpdateOrbitRotation();
    }

    public void OrbitDelta(float horizontalDegree, float verticalDegree) {
        _horizontalOrbit += horizontalDegree;
        _verticalOrbit += inverseVerticalOrbit ? -verticalDegree : verticalDegree;
        UpdateOrbitRotation();
    }

    [ContextMenu("Update Orbit Rotation")]
    private void UpdateOrbitRotation() {
        var orbitalTransposer = _virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        
        var offset = orbitalTransposer.m_FollowOffset;
        offset.y = Mathf.Sin(Mathf.Deg2Rad * _verticalOrbit) * orbitDistance;
        offset.z = -Mathf.Cos(Mathf.Deg2Rad * _verticalOrbit) * orbitDistance;
        orbitalTransposer.m_FollowOffset = offset;

        var heading = orbitalTransposer.m_Heading;
        heading.m_Bias = _horizontalOrbit;
        orbitalTransposer.m_Heading = heading;
    }
}