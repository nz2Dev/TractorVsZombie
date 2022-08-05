using UnityEngine;
using UnityEngine.EventSystems;

public class GrenaderCommander : MonoBehaviour {

    [SerializeField] private GroundObservable groundObservable;

    private CaravanSelection _grenaders;

    public void Activate(CaravanSelection greandersSelection) {
        _grenaders = greandersSelection;
        groundObservable.OnEvent += OnGroundEvent;
    }

    public void Deactivate() {
        groundObservable.OnEvent -= OnGroundEvent;
        _grenaders = null;
    }

    private void OnGroundEvent(GroundObservable.EventType eventType, PointerEventData eventData) {
        switch (eventType) {
            case GroundObservable.EventType.PointerDown:
            case GroundObservable.EventType.PointerDrag:
                AimAllGreanders(eventData.pointerCurrentRaycast.worldPosition);
                break;

            case GroundObservable.EventType.PointerUp:
                FireAllGreanders();
                break;

            default:
                throw new System.Exception();
        }
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
            greanderController.FireGrenade();
        }
    }

}