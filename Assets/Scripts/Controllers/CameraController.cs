using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    [SerializeField] private InputActionReference orbitAction;
    [SerializeField] private InputActionReference engageAction;
    [SerializeField] private InputActionReference zoomActionRef;
    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private CamerasManager cameras;

    private ICameraRig _controlledRig;
    private ICameraZoom _controlledZoom;
    private bool _orbiting;

    public string orbitActivationInputHint => engageAction.action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions | InputBinding.DisplayStringOptions.DontOmitDevice);
    public string orbitPerformingInputHint => orbitAction.action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontOmitDevice);
    public bool HasControledRig => _controlledRig != null;
    public bool HasControledZoom => _controlledZoom != null;
    public float CurrentControlledZoomLevel => HasControledZoom ? _controlledZoom.GetZoomLevel() : -1;

    public event Action<float> OnZoomLevelChanged;
    public event Action<ICameraRig> OnRigChanged;
    public event Action<ICameraZoom> OnZoomChanged;

    private void Awake() {
        cameras.OnVCamChanged += OnCameraChanged;

        caravanObserver.OnMembersChanged += (ctx) => {
            if (_controlledZoom != null) {
                _controlledZoom.SetZoomFactor(ctx.CountedLength);
            }
        };

        engageAction.action.performed += (ctx) => {
            _orbiting = ctx.ReadValueAsButton();
            if (_controlledRig != null && _controlledRig is IStatefulCameraRig statefulRig) {
                if (_orbiting) {
                    statefulRig.OnStartOrbiting();
                } else {
                    statefulRig.OnEndOrbiting();
                }
            }
        };

        zoomActionRef.action.performed += (ctx) => {
            if (_controlledZoom != null) {
                _controlledZoom.SetZoomLevel(_controlledZoom.GetZoomLevel() + ctx.ReadValue<float>());
                OnZoomLevelChanged?.Invoke(_controlledZoom.GetZoomLevel());
            }
        };
    }

    private void OnCameraChanged(GameObject disabled, GameObject enabled) {
        _controlledRig = enabled.GetComponent<ICameraRig>();
        OnRigChanged?.Invoke(_controlledRig);
        _controlledZoom = enabled.GetComponent<ICameraZoom>();
        OnZoomChanged?.Invoke(_controlledZoom);
    }

    public void SetZoomLevelManually(float zoomLevel) {
        if (_controlledZoom != null) {
            _controlledZoom.SetZoomLevel(zoomLevel);
        }
    }

    private void Update() {
        if (!_orbiting || _controlledRig == null)
            return;

        var input = orbitAction.action.ReadValue<Vector2>();
        var orbitXDelta = input.x / _controlledRig.OutputCamera.pixelWidth;
        var orbitYDelta = input.y / _controlledRig.OutputCamera.pixelHeight;

        _controlledRig.Orbit(
            orbitXDelta * Mathf.PI * Mathf.Rad2Deg,
            orbitYDelta * Mathf.PI * Mathf.Rad2Deg);
    }
}
