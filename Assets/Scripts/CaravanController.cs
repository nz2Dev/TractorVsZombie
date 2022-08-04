using System;
using UnityEngine;

[SelectionBase]
public class CaravanController : MonoBehaviour {
    
    [SerializeField] private CaravanSelection selection;

    private void Update() {
        if (selection.IsEmpty) {
            return;
        }

        foreach (var member in selection.SelectedMembers) {
            var selectedGrenaderController = member.GetComponentInChildren<GrenaderController>();
            if (selectedGrenaderController != null) {
                selectedGrenaderController.UpdateControl();
            }
        }
    }

}