using System;
using System.Collections.Generic;

using UnityEngine.UIElements;

public class SelectingView {

    private readonly VisualElement container;
    private readonly Dictionary<WeaponCategory, SelectionOptionDisplay> binding = new();

    public SelectingView(UIDocument uiDocument) {
        container = uiDocument.rootVisualElement.Q<VisualElement>("platformList");
        container.Clear();
    }

    internal void AddCategory(WeaponCategory key, VisualTreeAsset itemTemplate) {
        var display = new SelectionOptionDisplay(itemTemplate);
        container.Add(display.Root);
        binding[key] = display;
    }

    internal void RemoveCategory(WeaponCategory key) {
        binding.Remove(key, out var display);
        container.Remove(display.Root);
    }

    internal void ShowCategorySelected(WeaponCategory key) {
        var display = binding[key];
        display.SetSelected(true);
    }

    internal void ShowNoOptionSelected() {
        foreach (var display in binding.Values) {
            display.SetSelected(false);
        }
    }

    internal void UpdateCategoryOptionsCount(WeaponCategory key, int count) {
        var display = binding[key];
        display.SetAmount(count);
    }

}
