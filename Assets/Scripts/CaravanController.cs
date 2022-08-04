using System;
using UnityEngine;

[SelectionBase]
public class CaravanController : MonoBehaviour {
    
    [SerializeField] private CaravanSelection selection;

    public void OnPointerDown() {
        if (selection.IsEmpty) {
            return;
        }

        foreach (var member in selection.SelectedMembers) {
            var selectedGrenaderController = member.GetComponentInChildren<GrenaderController>();
            if (selectedGrenaderController != null) {
                selectedGrenaderController.OnPointerDown();
            }
        }
    }

    public void OnPointerStay() {
        if (selection.IsEmpty) {
            return;
        }

        foreach (var member in selection.SelectedMembers) {
            var selectedGrenaderController = member.GetComponentInChildren<GrenaderController>();
            if (selectedGrenaderController != null) {
                selectedGrenaderController.OnPointerStay();
            }
        }
    }

    public void OnPointerUp() {
        if (selection.IsEmpty) {
            return;
        }

        foreach (var member in selection.SelectedMembers) {
            var selectedGrenaderController = member.GetComponentInChildren<GrenaderController>();
            if (selectedGrenaderController != null) {
                selectedGrenaderController.OnPointerUp();
            }
        }
    }

}