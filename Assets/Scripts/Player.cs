using System;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private Tractor tractor;
    [SerializeField] private LayerMask groundLayerMask;

    private TrainElement _selection;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            var head = _selection != null ? _selection : tractor.GetComponent<TrainElement>();
            SelectTrainElement(TrainElementsUtils.FindNextElement(head));
        }

        if (_selection != null) {
            var selectedGrenader = _selection.GetComponentInChildren<Grenader>();
            if (selectedGrenader != null) {
                if (Input.GetMouseButton(0)) {
                    var camera = Camera.main;
                    var clickRay = camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(clickRay, out var hitInfo, float.MaxValue, groundLayerMask)) {
                        selectedGrenader.Aim(hitInfo.point);
                    }
                }

                if (Input.GetMouseButtonUp(0)) {
                    selectedGrenader.FireLastAimPoint();
                }
            } 
        }
    }

    private void SelectTrainElement(TrainElement trainElement) {
        if (_selection != null) {
            var unselectedSelectable = _selection.GetComponent<Selectable>();
            if (unselectedSelectable != null) {
                unselectedSelectable.SetSelected(false);
            }
        }

        _selection = trainElement;
        var newSelectable = _selection.GetComponent<Selectable>();
        if (newSelectable != null) {
            newSelectable.SetSelected(true);
        }
    }
}