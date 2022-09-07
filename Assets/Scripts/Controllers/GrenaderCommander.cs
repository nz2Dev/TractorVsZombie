using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GrenaderCommander : MonoBehaviour {

    [SerializeField] private InputActionReference grenadersEngage;
    [SerializeField] private InputActionReference fireModeToggle;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private GroundObservable groundObservable;
    [SerializeField] private bool singleFireMode;

    private Vector3 _aimPoint;
    private CaravanSelection _grenaders;
    private GrenaderController _singleFireGrenader;

    public event Action<bool> OnActiveStateChanged;
    
    public string ReloadActionBindingsName => reloadAction.action.GetBindingDisplayString();
    public string FireModeActionBindingsName => fireModeToggle.action.GetBindingDisplayString();
    public string fireActivationActionBindings => grenadersEngage.action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
    public string fireAimActionBindings => "position [mouse]";

    public void Activate(CaravanSelection greandersSelection) {
        _grenaders = greandersSelection;
        groundObservable.OnEvent += OnGroundEvent;
        OnActiveStateChanged?.Invoke(true);
    }

    public void Deactivate() {
        groundObservable.OnEvent -= OnGroundEvent;
        _grenaders = null;
        OnActiveStateChanged?.Invoke(false);
    }

    private void Update() {
        if (fireModeToggle.action.WasPerformedThisFrame()) {
            singleFireMode = !singleFireMode;
            _grenaders.ToggleSecondarySelection(singleFireMode);
            if (singleFireMode) {
                FindNextSingleGreander();
            }
        }

        if (reloadAction.action.WasPerformedThisFrame() && _grenaders != null) {
            foreach (var greander in _grenaders.SelectedMembers) {
                var ammo = greander.GetComponent<Ammo>();
                ammo.RefillFull();
            }
        }

        if (grenadersEngage.action.inProgress && grenadersEngage.action.WasPressedThisFrame()) {
            if (singleFireMode) {
                ActivateSingleGreander();
            } else {
                ActivateAllGreanders();
            }
        }

        if (_aimPoint != default) {
            if (singleFireMode) {
                AimSingleGreander();
            } else {
                AimAllGreanders();
            }
        }

        if (grenadersEngage.action.WasPerformedThisFrame() && grenadersEngage.action.WasReleasedThisFrame()) {
            _aimPoint = default;
            if (singleFireMode) {
                FireSingleGreander();
            } else {
                FireAllGreanders();
            }
        }
    }

    private void OnGroundEvent(GroundObservable.EventType eventType, PointerEventData eventData) {
        switch (eventType) {
            case GroundObservable.EventType.PointerDown:
                _aimPoint = eventData.pointerCurrentRaycast.worldPosition;
                break;

            case GroundObservable.EventType.PointerDrag:
                _aimPoint = eventData.pointerCurrentRaycast.worldPosition;
                break;

            case GroundObservable.EventType.PointerUp:
                _aimPoint = default;
                break;

            default:
                throw new System.Exception();
        }
    }

    private void FindNextSingleGreander() {
        _singleFireGrenader = _grenaders.SelectedMembers
                .Select((member) => member.GetComponent<GrenaderController>())
                .OrderBy((controller) => controller.TimeToReadynes)
                .FirstOrDefault();

        var greanderMember = _singleFireGrenader == null ? null : _singleFireGrenader.GetComponent<CaravanMember>();
        _grenaders.SetSecondarySelection(greanderMember);
    }

    private void ActivateSingleGreander() {
        if (_singleFireGrenader == null) {
            FindNextSingleGreander();
        }

        if (_singleFireGrenader != null && !_singleFireGrenader.IsActivated) {
            _singleFireGrenader.Activate(_aimPoint);
        }
    }

    private void AimSingleGreander() {
        if (_singleFireGrenader != null) {
            _singleFireGrenader.Aim(_aimPoint);
        }
    }

    private void FireSingleGreander() {
        if (_singleFireGrenader != null) {
            _singleFireGrenader.Fire();
            FindNextSingleGreander();
        }
    }

    private void FindAimLine(out Vector3 lineStart, out Vector3 lineStep) {
        var membersPositionSum = Vector3.zero;
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            membersPositionSum += greanderMember.transform.position;
        }

        var lineStepLength = 2f;
        var membersCenter = membersPositionSum / _grenaders.SelectedCount;
        var lineDirection = Vector3.Cross((_aimPoint - membersCenter).normalized, Vector3.up);
        lineStep = lineDirection * lineStepLength;
        var lineTotal = (_grenaders.SelectedCount - 1) * lineStep;
        lineStart = _aimPoint - lineTotal / 2f;
    }

    private void ActivateAllGreanders() {
        FindAimLine(out var lineStart, out var lineStep);
        var lineCurrent = lineStart;
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderController>();
            greanderController.Activate(lineCurrent);
            lineCurrent += lineStep;
        }
    }

    private void AimAllGreanders() {
        FindAimLine(out var lineStart, out var lineStep);
        var lineCurrent = lineStart;
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderController>();
            greanderController.Aim(lineCurrent);
            lineCurrent += lineStep;
        }
    }

    private void FireAllGreanders() {
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderController>();
            greanderController.Fire();
        }
    }

}