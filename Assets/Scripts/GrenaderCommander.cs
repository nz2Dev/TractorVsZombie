using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrenaderCommander : MonoBehaviour {

    [SerializeField] private GroundObservable groundObservable;
    [SerializeField] private bool singleFireMode;

    private CaravanSelection _grenaders;
    private GrenaderController _singleFireGrenader;

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
    }

    private void OnGroundEvent(GroundObservable.EventType eventType, PointerEventData eventData) {
        switch (eventType) {
            case GroundObservable.EventType.PointerDown:
            case GroundObservable.EventType.PointerDrag:
                var aimPoint = eventData.pointerCurrentRaycast.worldPosition;
                if (singleFireMode) {
                    AimSingleGreander(aimPoint);
                } else {
                    AimAllGreanders(aimPoint);
                }
                break;

            case GroundObservable.EventType.PointerUp:
                if (singleFireMode) {
                    FireSingleGreander();
                } else {
                    FireAllGreanders();
                }
                break;

            default:
                throw new System.Exception();
        }
    }

    private void AimSingleGreander(Vector3 point) {
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

        _singleFireGrenader.AimGreander(point);
    }

    private void FireSingleGreander() {
        if (_singleFireGrenader == null) {
            return;
        }

        _singleFireGrenader.FireGreandeAtLastAimed();
        _singleFireGrenader = null;
    }

    private void AimAllGreanders(Vector3 point) {
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderController>();
            greanderController.AimGreander(point);
        }
    }

    private void FireAllGreanders() {
        foreach (var greanderMember in _grenaders.SelectedMembers) {
            var greanderController = greanderMember.GetComponent<GrenaderController>();
            greanderController.FireGreandeAtLastAimed();
        }
    }

}