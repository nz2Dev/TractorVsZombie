using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaravanSelection : MonoBehaviour {

    [SerializeField] private Color primarySelectionColor;
    [SerializeField] private Color secondarySelectionColor;
    [SerializeField] private Color secondaryUnselectedColor;

    private CaravanMember[] _selected = new CaravanMember[0];
    private CaravanMember _secondarySelected;
    private bool _secondarySelectionEnabled;

    public IEnumerable<CaravanMember> SelectedMembers => _selected.Where((member) => member != null);
    public bool IsGrenaders => !IsEmpty && _selected.Any((member) => member.GetComponent<GrenaderController>() != null);
    public bool IsEmpty => _selected == null || _selected.Length == 0;

    public void SetSelection(CaravanMember[] members) {
        foreach (var previouslySelected in SelectedMembers) {
            if (previouslySelected != null) {
                var selectable = previouslySelected.GetComponent<Selectable>();
                if (selectable != null) {
                    selectable.PrimarySelection(false);
                }
            }
        }

        _selected = members;
        foreach (var selectedMember in members) {
            var selectable = selectedMember.GetComponent<Selectable>();
            if (selectable != null) {
                selectable.SetSelectionColors(primarySelectionColor, secondarySelectionColor, secondaryUnselectedColor);
                selectable.PrimarySelection(true);
            }
        }
    }

    public void ToggleSecondarySelection(bool secondarySelection) {
        _secondarySelectionEnabled = secondarySelection;
        UpdateSecondarySelection();
    }

    public void SetSecondarySelection(CaravanMember member) {
        _secondarySelected = member;
        UpdateSecondarySelection();
    }

    private void UpdateSecondarySelection() {
        foreach (var member in SelectedMembers) {
            var selectable = member.GetComponent<Selectable>();
            selectable.SecondarySelection(_secondarySelectionEnabled, _secondarySelected == member);
        }
    }
}