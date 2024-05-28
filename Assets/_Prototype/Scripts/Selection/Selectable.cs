using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

    [SerializeField] private SpriteRenderer selectionRenderer;

    private Color _primarySelectionColor;
    private Color _secondarySelectionColor;
    private Color _secondaryUnselectionColor;

    private bool _secondarySelectionEnabled;
    private bool _secondarySelected;

    public void SetSelectionColors(Color primarySelectionColor, Color secondarySelectionColor, Color secondaryUnselectionColor) {
        _primarySelectionColor = primarySelectionColor;
        _secondarySelectionColor = secondarySelectionColor;
        _secondaryUnselectionColor = secondaryUnselectionColor;
        UpdateSelectionColor();
    }

    public void PrimarySelection(bool selected) {
        selectionRenderer.gameObject.SetActive(selected);
        UpdateSelectionColor();
    }

    public void SecondarySelection(bool enabled, bool selected) {
        _secondarySelectionEnabled = enabled;
        _secondarySelected = selected;
        UpdateSelectionColor();
    }

    private void UpdateSelectionColor() {
        if (_secondarySelectionEnabled) {
            selectionRenderer.color = _secondarySelected ? _secondarySelectionColor : _secondaryUnselectionColor;
            return;
        }

        selectionRenderer.color = _primarySelectionColor;
    }
}
