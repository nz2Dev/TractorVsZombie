using System;
using UnityEngine;

[SelectionBase]
public class CaravanMemberSelection : MonoBehaviour {
    [SerializeField] private Tractor tractor;

    private CaravanMember[] _selection = new CaravanMember[0];

    public void SetMultiSelection(CaravanMember[] members) {
        foreach (var unselectedMember in _selection) {
            if (unselectedMember != null) {
                var unselectedSelectable = unselectedMember.GetComponent<Selectable>();
                if (unselectedSelectable != null) {
                    unselectedSelectable.SetSelected(false);
                }
            }
        }

        _selection = members;
        foreach (var selectedMember in members) {
            var newSelectable = selectedMember.GetComponent<Selectable>();
            if (newSelectable != null) {
                newSelectable.SetSelected(true);
            }
        }
    }

    private void Update() {
        if (_selection != null) {
            foreach (var member in _selection) {
                if (member != null) {
                    var selectedGrenaderController = member.GetComponentInChildren<GrenaderController>();
                    if (selectedGrenaderController != null) {
                        selectedGrenaderController.UpdateControl();
                    }
                }
            }
        }
    }

}