using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class SelectingView {

    private readonly VisualElement container;
    private readonly Dictionary<WeaponCategory, SelectionOptionDisplay> binding = new();

    public SelectingView(UIDocument uiDocument) {
        container = uiDocument.rootVisualElement.Q<VisualElement>("platformList");
        container.Clear();
    }

    internal void AddCategory(WeaponCategory key, VisualTreeAsset itemTemplate, Sprite weaponIcon) {
        var display = new SelectionOptionDisplay(itemTemplate);
        display.SetImage(weaponIcon);
        container.Add(display.Root);
        binding[key] = display;
    }

    internal void RemoveCategory(WeaponCategory key) {
        binding.Remove(key, out var display);
        container.Remove(display.Root);
    }

    internal void ShowCategorySelected(WeaponCategory key) {
        var display = binding[key];
        display.SetAccentState(SelectionOptionDisplay.AccentState.Selected);
    }

    internal void ShowNoOptionSelected() {
        foreach (var display in binding.Values) {
            display.SetAccentState(SelectionOptionDisplay.AccentState.None);
        }
    }

    internal void UpdateCategoryOptionsCount(WeaponCategory key, int count) {
        var display = binding[key];
        display.SetAmount(count);
    }

}
