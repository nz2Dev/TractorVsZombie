using System;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private Tractor tractor;

    private TrainElement _selection;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            var head = _selection != null ? _selection : tractor.GetComponent<TrainElement>();
            SelectTrainElement(TrainElementsUtils.FindNextElement(head));
        }

        if (_selection != null) {
            var selectedGrenaderController = _selection.GetComponentInChildren<GrenaderController>();
            if (selectedGrenaderController != null) {
                selectedGrenaderController.UpdateControl();
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