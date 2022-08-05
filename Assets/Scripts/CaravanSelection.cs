using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaravanSelection : MonoBehaviour {
    private CaravanMember[] _selected = new CaravanMember[0];

    public IEnumerable<CaravanMember> SelectedMembers => _selected.Where((member) => member != null);

    public bool IsEmpty => _selected == null || _selected.Length == 0;

    public bool IsGrenaders => !IsEmpty && _selected.Any((member) => member.GetComponent<GrenaderController>() != null);

    public void SetMultiSelection(CaravanMember[] members) {
        foreach (var previouslySelected in SelectedMembers) {
            if (previouslySelected != null) {
                var selectable = previouslySelected.GetComponent<Selectable>();
                if (selectable != null) {
                    selectable.SetSelected(false);
                }
            }
        }

        _selected = members;
        foreach (var selectedMember in members) {
            var selectable = selectedMember.GetComponent<Selectable>();
            if (selectable != null) {
                selectable.SetSelected(true);
            }
        }
    }
}