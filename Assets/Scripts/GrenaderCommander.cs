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

    private void AimSingleGreander() {
        if (_singleFireGrenader == null) {
            var nextGreander = _grenaders.SelectedMembers
                .Select((member) => member.GetComponent<GrenaderController>())
                .OrderBy((controller) => controller.TimeToReadynes)
                .FirstOrDefault();

            if (nextGreander == null) {
                return;
            }

            _singleFireGrenader = nextGreander;
        }

        _singleFireGrenader.AimGreander(_aimPoint);
    }

    private void FireSingleGreander() {
        if (_singleFireGrenader == null) {
            return;
        }

        _singleFireGrenader.FireGreandeAtLastAimed();
        _singleFireGrenader = null;
    }

    private void AimAllGreanders() {
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderController>();
            greanderController.AimGreander(_aimPoint);
        }
    }

    private void FireAllGreanders() {
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderController>();
            greanderController.FireGreandeAtLastAimed();
        }
    }

}