using System;
using System.ComponentModel.Design;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FrameTransposerCameraRig : MonoBehaviour, ICameraStateListener, ICameraRig {

    [SerializeField] private float orbitSpeedMultiplier = 1;
    [SerializeField] private bool inverseVerticalOrbit = true;
    [SerializeField][Range(-180, 180)] private float initialOrbitHorizontally = 0;
    [SerializeField][Range(0, 89)] private float initialOrbitVertically = 75f;

    private CinemachineVirtualCamera _vcam;
    private Camera _targetCamera;
    private float _horizontalOrbit;
    private float _verticalOrbit;

    Camera ICameraRig.OutputCamera => _targetCamera;

    private void Awake() {
        _vcam = GetComponent<CinemachineVirtualCamera>();
        ResetOrbitRotation();        
    }

    public void OnActiveStateChanged(bool activeState) {
        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(_vcam);
        _targetCamera = brain == null ? null : brain.OutputCamera;
    }

    void ICameraRig.Orbit(float horizontalDegree, float verticalDegree) {
        _horizontalOrbit += horizontalDegree * orbitSpeedMultiplier;
        _verticalOrbit += (inverseVerticalOrbit ? -verticalDegree : verticalDegree) * orbitSpeedMultiplier;
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