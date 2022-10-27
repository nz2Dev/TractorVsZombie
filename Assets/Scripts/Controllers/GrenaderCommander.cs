using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CaravanGroupObserver))]
public class GrenaderCommander : MonoBehaviour {

    [SerializeField] private InputActionReference engageAction;
    [SerializeField] private InputActionReference fireModeToggle;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private InputActionReference aimAction;
    [SerializeField] private WorldRaycaster worldRaycaster;
    [SerializeField] private String grenaderTag = "Grenader";
    [SerializeField] private bool singleFireMode;

    private Vector3 _aimPoint;
    private CaravanSelection _selection;
    private GrenaderOperator _singleFireGrenader;
    private CaravanGroupObserver _caravanGroupObserver;

    public event Action<bool> OnActiveStateChanged;

    public string ReloadActionBindingsName => reloadAction.action.GetBindingDisplayString();
    public string FireModeActionBindingsName => fireModeToggle.action.GetBindingDisplayString();
    public string enageActionBindings => engageAction.action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
    public string aimActionBindings => "position [mouse]";

    private void Awake() {
        _caravanGroupObserver = GetComponent<CaravanGroupObserver>();
    }
    public void Activate(CaravanObservable caravan, CaravanSelection selection) {
        _selection = selection;
        _caravanGroupObserver.Subscribe(caravan, grenaderTag, OnGrenadersGroupChanged);
        OnActiveStateChanged?.Invoke(true);
    }

    public void Deactivate() {
        if (_selection == null) {
            return;
        }

        _selection = null;
        _caravanGroupObserver.UnsubscribeFromCaravan();

        foreach (var grenaderMember in _caravanGroupObserver.GroupMembers) {
            var grenaderOperator = grenaderMember.GetComponent<GrenaderOperator>();
            grenaderOperator.OnReloaded -= UpdateFireModeState;
            grenaderOperator.Cancel();
        }

        OnActiveStateChanged?.Invoke(false);
    }

    private void OnGrenadersGroupChanged(CaravanGroupObserver observer) {
        _selection.SetSelection(observer.GroupMembers);

        foreach (var grenaderMember in observer.GroupMembers) {
            var grenaderOperator = grenaderMember.GetComponent<GrenaderOperator>();
            // potential leak, as group memebers can be changed not because of destruction, 
            // and we don't remove OnReload subscription from them, only from the last recorded group
            grenaderOperator.OnReloaded -= UpdateFireModeState;
            grenaderOperator.OnReloaded += UpdateFireModeState;
        }

        UpdateFireModeState();
    }

    private void Update() {
        if (reloadAction.action.WasPerformedThisFrame() && _selection != null) {
            foreach (var greander in _selection.SelectedMembers) {
                var ammo = greander.GetComponent<Ammo>();
                ammo.RefillFull();
            }
        }

        if (_selection == null) {
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
            _selection.ToggleSecondarySelection(true);
            FindNextSingleGreander();

            foreach(var member in _selection.SelectedMembers) {
                var grenader = member.GetComponent<GrenaderOperator>();
                if (grenader != _singleFireGrenader) {
                    grenader.Cancel();
                }
            }
        } else {
            _selection.ToggleSecondarySelection(false);
        }
    }

    private void FindNextSingleGreander() {
        if (_singleFireGrenader != null && _singleFireGrenader.ReadyForActivation) {
            return;
        }

        _singleFireGrenader = _selection.SelectedMembers
                .Select((member) => member.GetComponent<GrenaderOperator>())
                .Where((controller) => controller.ReadyForActivation)
                .OrderBy((controller) => controller.TimeToReadynes)
                .FirstOrDefault();

        var greanderMember = _singleFireGrenader == null ? null : _singleFireGrenader.GetComponent<CaravanMember>();
        _selection.SetSecondarySelection(greanderMember);
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
        foreach (var greanderMember in _selection.SelectedMembers) {
            membersPositionSum += greanderMember.transform.position;
        }

        var lineStepLength = 2f;
        var membersCenter = membersPositionSum / _selection.SelectedCount;
        var lineDirection = Vector3.Cross((_aimPoint - membersCenter).normalized, Vector3.up);
        lineStep = lineDirection * lineStepLength;
        var lineTotal = (_selection.SelectedCount - 1) * lineStep;
        lineStart = _aimPoint - lineTotal / 2f;
    }

    private void AimAllGreanders() {
        FindAimLine(out var lineStart, out var lineStep);
        var lineCurrent = lineStart;

        foreach (var greanderMember in _selection.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Aim(lineCurrent);

            lineCurrent += lineStep;
        }
    }

    private void FireAllGreanders() {
        foreach (var greanderMember in _selection.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Fire();
        }
    }

}