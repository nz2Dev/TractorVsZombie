using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CaravanGroupObserver))]
public class CaravanGrenadersCommander : MonoBehaviour, ICaravanGroupCommander {

    [SerializeField] private InputActionReference engageAction;
    [SerializeField] private InputActionReference fireModeToggle;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private InputActionReference aimAction;
    [SerializeField] private WorldRaycaster worldRaycaster;
    [SerializeField] private ColorSchema selectionColorSchema;
    [SerializeField] private String controlGroupTag;
    [SerializeField] private bool singleFireMode;

    private Vector3 _aimPoint;
    private GrenaderOperator _singleFireGrenader;
    private CaravanGroupObserver _groupObserver;
    private CaravanSelection _selection;

    public event Action<bool> OnActiveStateChanged;

    CommanderGroupInfo ICaravanGroupCommander.GroupInfo => new CommanderGroupInfo {name = controlGroupTag, length = _groupObserver.GroupSize };
    public string ReloadActionBindingsName => reloadAction.action.GetBindingDisplayString();
    public string FireModeActionBindingsName => fireModeToggle.action.GetBindingDisplayString();
    public string enageActionBindings => engageAction.action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
    public string aimActionBindings => "position [mouse]";

    private void Awake() {
        _groupObserver = GetComponent<CaravanGroupObserver>();
    }

    void ICaravanGroupCommander.Subscribe(CaravanObservable caravan, Action onGroupChanged) {
        _groupObserver.Subscribe(caravan, controlGroupTag, (o) => {
            ConfigureGroupCallbacks();
            ConfigureGroupSelection();
            UpdateFireModeState();
            onGroupChanged?.Invoke();
        });
    }

    void ICaravanGroupCommander.Activate(CaravanSelection selection) {
        _selection = selection;
        _selection.SetColorSchema(selectionColorSchema);

        ConfigureGroupSelection();
        UpdateFireModeState();
        
        OnActiveStateChanged?.Invoke(true);
    }

    void ICaravanGroupCommander.Deactivate() {
        _selection = null;

        foreach (var grenaderMember in _groupObserver.GroupMembers) {
            var grenaderOperator = grenaderMember.GetComponent<GrenaderOperator>();
            grenaderOperator.OnReloaded -= UpdateFireModeState;
            grenaderOperator.Cancel();
        }

        OnActiveStateChanged?.Invoke(false);
    }

    private void ConfigureGroupSelection() {
        if (_selection != null) {
            _selection.SetSelection(_groupObserver.GroupMembers);
            
            var singleFireGreanderMember = _singleFireGrenader == null ? null : _singleFireGrenader.GetComponent<CaravanMember>();
            _selection.SetSecondarySelection(singleFireGreanderMember);
        }
    }

    private void ConfigureGroupCallbacks() {
        foreach (var grenaderMember in _groupObserver.GroupMembers) {
            var grenaderOperator = grenaderMember.GetComponent<GrenaderOperator>();
            grenaderOperator.OnReloaded -= UpdateFireModeState;
            grenaderOperator.OnReloaded += UpdateFireModeState;
        }
    }

    private void UpdateFireModeState() {
        if (_selection != null) {
            _selection.ToggleSecondarySelection(singleFireMode);
        }

        if (singleFireMode) {
            FindNextSingleGreander();

            foreach (var member in _groupObserver.GroupMembers) {
                var grenader = member.GetComponent<GrenaderOperator>();
                if (grenader != _singleFireGrenader) {
                    grenader.Cancel();
                }
            }
        }
    }

    private void Update() {
        if (reloadAction.action.WasPerformedThisFrame() && _groupObserver != null) {
            foreach (var greander in _groupObserver.GroupMembers) {
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
        }
    }

    private void FindNextSingleGreander() {
        if (_singleFireGrenader != null && _singleFireGrenader.ReadyForActivation) {
            return;
        }

        _singleFireGrenader = _groupObserver.GroupMembers
                .Select((member) => member.GetComponent<GrenaderOperator>())
                .Where((controller) => controller.ReadyForActivation)
                .OrderBy((controller) => controller.TimeToReadynes)
                .FirstOrDefault();

        if (_selection != null) {
            var greanderMember = _singleFireGrenader == null ? null : _singleFireGrenader.GetComponent<CaravanMember>();
            _selection.SetSecondarySelection(greanderMember);
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
            UpdateFireModeState();
        }
    }

    private void AimAllGreanders() {
        FindAimLine(out var lineStart, out var lineStep);
        var lineCurrent = lineStart;

        foreach (var greanderMember in _groupObserver.GroupMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Aim(lineCurrent);

            lineCurrent += lineStep;
        }
    }

    private void FireAllGreanders() {
        foreach (var greanderMember in _groupObserver.GroupMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Fire();
        }
    }

    private void FindAimLine(out Vector3 lineStart, out Vector3 lineStep) {
        var membersPositionSum = Vector3.zero;
        foreach (var greanderMember in _groupObserver.GroupMembers) {
            membersPositionSum += greanderMember.transform.position;
        }

        var lineStepLength = 2f;
        var membersCenter = membersPositionSum / _groupObserver.GroupSize;
        var lineDirection = Vector3.Cross((_aimPoint - membersCenter).normalized, Vector3.up);
        lineStep = lineDirection * lineStepLength;
        var lineTotal = (_groupObserver.GroupSize - 1) * lineStep;
        lineStart = _aimPoint - lineTotal / 2f;
    }
}