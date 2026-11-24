using System;
using System.Collections.Generic;

using UnityEngine.UIElements;

public class SelectingView {

    private readonly VisualElement container;
    private readonly Dictionary<WeaponCategory, Label> binding = new();

    public SelectingView(UIDocument uiDocument) {
        container = uiDocument.rootVisualElement.Q<VisualElement>("platformList");
        container.Clear();
    }

    internal void AddCategory(WeaponCategory key, string categoryName) {
        MakeLabel(out var label);
        binding[key] = label;
        label.text = categoryName;
    }

    internal void RemoveCategory(WeaponCategory key) {
        if (!binding.Remove(key, out var label))
            return;
        container.Remove(label);
    }

    internal void ShowCategorySelected(WeaponCategory key) {
        if (binding.TryGetValue(key, out var label))
            label.AddToClassList("selected-label");
    }

    internal void ShowNoOptionSelected() {
        foreach (var label in binding.Values) {
            label.RemoveFromClassList("selected-label");
        }
    }

    internal void UpdateCategoryOptionsCount(WeaponCategory key, int count) {
        if (binding.TryGetValue(key, out var label)) {
            label.text = key.name + " " + count;
        }
    }

    private void MakeLabel(out Label created) {
        created = new Label();
        created.AddToClassList("list-text");
        container.Add(created);
    }

}
