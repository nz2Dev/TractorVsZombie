using System;
using System.ComponentModel.Design;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FrameTransposerCameraOrbitController : MonoBehaviour, ICameraController {

    [SerializeField] private InputActionReference orbitAction; 
    [SerializeField] private InputActionReference engageAction;
    [SerializeField] private float orbitSpeedMultiplier = 1;
    [SerializeField] private bool inverseVerticalOrbit = true;
    [SerializeField][Range(-180, 180)] private float initialOrbitHorizontally = 0;
    [SerializeField][Range(0, 89)] private float initialOrbitVertically = 75f;

    private CinemachineVirtualCamera _vcam;
    private Camera _targetCamera;
    private float _horizontalOrbit;
    private float _verticalOrbit;
    private bool _orbiting;
    private bool _active;

    private void Awake() {
        _vcam = GetComponent<CinemachineVirtualCamera>();

        ResetOrbitRotation();

        engageAction.action.performed += (ctx) => {
            _orbiting = ctx.ReadValueAsButton();
        };
    }

    public void OnActiveStateChanged(bool activeState) {
        _active = activeState;
        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(_vcam);
        _targetCamera = brain == null ? null : brain.OutputCamera;
    }

    private void Update() {
        if (!_active || !_orbiting)
            return;
        
        var input = orbitAction.action.ReadValue<Vector2>();
        var orbitXDelta = input.x / _targetCamera.pixelWidth;
        var orbitYDelta = input.y / _targetCamera.pixelHeight;

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