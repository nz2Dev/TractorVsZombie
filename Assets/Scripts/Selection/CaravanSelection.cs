using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ColorSchema {
    [SerializeField] public Color primarySelectionColor;
    [SerializeField] public Color secondarySelectionColor;
    [SerializeField] public Color secondaryUnselectedColor;
}

public class CaravanSelection : MonoBehaviour {

    [SerializeField] private ColorSchema defaultColorSchema;

    private CaravanMember[] _selected = new CaravanMember[0];
    private CaravanMember _secondarySelected;
    private bool _secondarySelectionEnabled;
    private ColorSchema _colorSchema;

    public IEnumerable<CaravanMember> SelectedMembers => _selected.Where((member) => member != null);
    public int SelectedCount => _selected.Length;
    public bool IsEmpty => _selected == null || _selected.Length == 0;

    private void Awake() {
        SetColorSchema(defaultColorSchema);
    }

    public void SetColorSchema(ColorSchema? schema) {
        _colorSchema = schema == null ? defaultColorSchema : schema.Value;
    }

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
                selectable.SetSelectionColors(
                    _colorSchema.primarySelectionColor, 
                    _colorSchema.secondarySelectionColor, 
                    _colorSchema.secondaryUnselectedColor);
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