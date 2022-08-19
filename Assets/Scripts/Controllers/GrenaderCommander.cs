using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrenaderCommander : MonoBehaviour {

    [SerializeField] private GroundObservable groundObservable;
    [SerializeField] private bool singleFireMode;

    private CaravanSelection _grenaders;
    private GrenaderController _singleFireGrenader;

    private Vector3 _aimPoint;

    public void Activate(CaravanSelection greandersSelection) {
        _grenaders = greandersSelection;
        groundObservable.OnEvent += OnGroundEvent;
    }

    public void Deactivate() {
        groundObservable.OnEvent -= OnGroundEvent;
        _grenaders = null;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            singleFireMode = !singleFireMode;
            _grenaders.ToggleSecondarySelection(singleFireMode);
            if (singleFireMode) {
                FindNextSingleGreander();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && _grenaders != null) {
            foreach (var greander in _grenaders.SelectedMembers) {
                var ammo = greander.GetComponent<Ammo>();
                ammo.RefillFull();
            }
        }

        if (_aimPoint != default) {
            if (singleFireMode) {
                AimSingleGreander();
            } else {
                AimAllGreanders();
            }
        }
    }

    private void OnGroundEvent(GroundObservable.EventType eventType, PointerEventData eventData) {
        switch (eventType) {
            case GroundObservable.EventType.PointerDown:
                _aimPoint = eventData.pointerCurrentRaycast.worldPosition;
                if (singleFireMode) {
                    ActivateSingleGreander();
                } else {
                    ActivateAllGreanders();
                }
                break;

            case GroundObservable.EventType.PointerDrag:
                _aimPoint = eventData.pointerCurrentRaycast.worldPosition;
                break;

            case GroundObservable.EventType.PointerUp:
                if (singleFireMode) {
                    FireSingleGreander();
                } else {
                    FireAllGreanders();
                }
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