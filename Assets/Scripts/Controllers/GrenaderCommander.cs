using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GrenaderCommander : MonoBehaviour {

    [SerializeField] private InputActionReference engageAction;
    [SerializeField] private InputActionReference fireModeToggle;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private InputActionReference aimAction;
    [SerializeField] private WorldRaycaster worldRaycaster;
    [SerializeField] private bool singleFireMode;

    private Vector3 _aimPoint;
    private CaravanSelection _grenaders;
    private GrenaderOperator _singleFireGrenader;

    public event Action<bool> OnActiveStateChanged;

    public string ReloadActionBindingsName => reloadAction.action.GetBindingDisplayString();
    public string FireModeActionBindingsName => fireModeToggle.action.GetBindingDisplayString();
    public string enageActionBindings => engageAction.action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
    public string aimActionBindings => "position [mouse]";

    public void Activate(CaravanSelection greandersSelection) {
        _grenaders = greandersSelection;
        
        UpdateFireModeState();
        foreach (var grenader in _grenaders.SelectedMembers) {
            var controller = grenader.GetComponent<GrenaderOperator>();
            controller.OnReloaded += () => {
                UpdateFireModeState();
            };
        }

        OnActiveStateChanged?.Invoke(true);
    }

    public void Deactivate() {
        if (_grenaders != null && _grenaders.SelectedMembers != null) {
            foreach (var member in _grenaders.SelectedMembers) {
                var grenader = member.GetComponent<GrenaderOperator>();
                grenader.Cancel();
            }
        }

        _grenaders = null;
        OnActiveStateChanged?.Invoke(false);
    }

    private void Update() {
        if (reloadAction.action.WasPerformedThisFrame() && _grenaders != null) {
            foreach (var greander in _grenaders.SelectedMembers) {
                var ammo = greander.GetComponent<Ammo>();
                ammo.RefillFull();
            }
        }

        if (_grenaders == null) {
            return;
        }

        if (worldRaycaster.RaycastGroundPoint(aimAction.action.ReadValue<Vector2>(), out var groundPoint)) {
            _aimPoint = groundPoint;
        }

        if (fireModeToggle.action.WasPerformedThisFrame()) {
            singleFireMode = !singleFireMode;
            UpdateFireModeState();
        }

        if (_aimPoint != default) {
            if (singleFireMode) {
                AimSingleGreander();
            } else {
                AimAllGreanders();
            }
        }

        if (engageAction.action.WasPerformedThisFrame() && engageAction.action.WasReleasedThisFrame()) {
            _aimPoint = default;
            if (singleFireMode) {
                FireSingleGreander();
            } else {
                FireAllGreanders();
            }

            UpdateFireModeState();
        }
    }

    private void UpdateFireModeState() {
        if (singleFireMode) {
            _grenaders.ToggleSecondarySelection(true);
            FindNextSingleGreander();

            foreach(var member in _grenaders.SelectedMembers) {
                var grenader = member.GetComponent<GrenaderOperator>();
                if (grenader != _singleFireGrenader) {
                    grenader.Cancel();
                }
            }
        } else {
            _grenaders.ToggleSecondarySelection(false);
        }
    }

    private void FindNextSingleGreander() {
        if (_singleFireGrenader != null && _singleFireGrenader.ReadyForActivation) {
            return;
        }

        _singleFireGrenader = _grenaders.SelectedMembers
                .Select((member) => member.GetComponent<GrenaderOperator>())
                .Where((controller) => controller.ReadyForActivation)
                .OrderBy((controller) => controller.TimeToReadynes)
                .FirstOrDefault();

        var greanderMember = _singleFireGrenader == null ? null : _singleFireGrenader.GetComponent<CaravanMember>();
        _grenaders.SetSecondarySelection(greanderMember);
    }

    private void AimSingleGreander() {
        if (_singleFireGrenader != null) {
            _singleFireGrenader.Aim(_aimPoint);
        }
    }

    private void FireSingleGreander() {
        if (_singleFireGrenader != null) {
            _singleFireGrenader.Fire();
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

    private void AimAllGreanders() {
        FindAimLine(out var lineStart, out var lineStep);
        var lineCurrent = lineStart;

        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Aim(lineCurrent);

            lineCurrent += lineStep;
        }
    }

    private void FireAllGreanders() {
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Fire();
        }
    }

}