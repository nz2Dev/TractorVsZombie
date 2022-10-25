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
    [SerializeField] private NamedPrototypePopulator aimOutlinePopulator;
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
        
        UpdateActivatedGrenaders();
        foreach (var grenader in _grenaders.SelectedMembers) {
            var controller = grenader.GetComponent<GrenaderOperator>();
            controller.OnReloaded += () => {
                UpdateActivatedGrenaders();
            };
        }

        OnActiveStateChanged?.Invoke(true);
    }

    public void Deactivate() {
        _grenaders = null;
        OnActiveStateChanged?.Invoke(false);
    }

    private void Update() {
        if (worldRaycaster.RaycastGroundPoint(aimAction.action.ReadValue<Vector2>(), out var groundPoint)) {
            _aimPoint = groundPoint;
        }

        if (reloadAction.action.WasPerformedThisFrame() && _grenaders != null) {
            foreach (var greander in _grenaders.SelectedMembers) {
                var ammo = greander.GetComponent<Ammo>();
                ammo.RefillFull();
            }
        }

        if (fireModeToggle.action.WasPerformedThisFrame()) {
            singleFireMode = !singleFireMode;
            UpdateActivatedGrenaders();
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

            UpdateActivatedGrenaders();
        }
    }

    private void UpdateActivatedGrenaders() {
        if (singleFireMode) {
            aimOutlinePopulator.Clear();
            _grenaders.ToggleSecondarySelection(true);
            
            FindNextSingleGreander();
            ActivateSingleGreander();
        } else {
            aimOutlinePopulator.Clear();
            _grenaders.ToggleSecondarySelection(false);

            ActivateAllGreanders();
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

    private void ActivateSingleGreander() {
        if (_singleFireGrenader != null) {
            _singleFireGrenader.Activate();

            if (_singleFireGrenader.IsActivated) {
                var aimOutline = aimOutlinePopulator.GetOrCreateChild<AimOutline>(0);
                aimOutline.StartOutlining(_singleFireGrenader.LauncherPosition, _aimPoint, _singleFireGrenader.ExplosionRadius);
            }
        }
    }

    private void AimSingleGreander() {
        if (_singleFireGrenader != null) {
            _singleFireGrenader.Aim(_aimPoint);

            var aimOutline = aimOutlinePopulator.GetOrCreateChild<AimOutline>(0);
            aimOutline.OutlineTarget(_singleFireGrenader.LauncherPosition, _aimPoint);
        }
    }

    private void FireSingleGreander() {
        if (_singleFireGrenader != null) {
            _singleFireGrenader.Fire();
            aimOutlinePopulator.DestroyChild(0);
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
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Activate();

            if (greanderController.IsActivated) {
                var aimOutline = aimOutlinePopulator.GetOrCreateChild<AimOutline>(greanderController.GetInstanceID());
                aimOutline.StartOutlining(greanderController.LauncherPosition, lineCurrent, greanderController.ExplosionRadius);
            }

            lineCurrent += lineStep;
        }
    }

    private void AimAllGreanders() {
        FindAimLine(out var lineStart, out var lineStep);
        var lineCurrent = lineStart;
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Aim(lineCurrent);

            var aimOutline = aimOutlinePopulator.GetOrCreateChild<AimOutline>(greanderController.GetInstanceID());
            aimOutline.OutlineTarget(greanderController.LauncherPosition, lineCurrent);

            lineCurrent += lineStep;
        }
    }

    private void FireAllGreanders() {
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderOperator>();
            greanderController.Fire();
        }

        aimOutlinePopulator.Clear();
    }

}