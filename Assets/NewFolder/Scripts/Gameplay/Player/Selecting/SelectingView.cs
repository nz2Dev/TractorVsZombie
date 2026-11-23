using System.Collections.Generic;

using UnityEngine.UIElements;

public class SelectingView {

    private readonly VisualElement container;
    private readonly Dictionary<SelectionOption, Label> binding = new();

    public SelectingView(UIDocument uiDocument) {
        container = uiDocument.rootVisualElement.Q<VisualElement>("platformList");
        container.Clear();
    }

    internal void AddOption(SelectionOption option) {
        MakeLabel(out var label);
        binding[option] = label;
        label.text = "Option";
    }

    internal void RemoveOption(SelectionOption key) {
        if (!binding.Remove(key, out var label))
            return;
        container.Remove(label);
    }

    internal void ShowOptionSelected(SelectionOption key) {
        if (binding.TryGetValue(key, out var label))
            label.AddToClassList("selected-label");
    }

    internal void ShowNoOptionSelected() {
        foreach (var label in binding.Values) {
            label.RemoveFromClassList("selected-label");
        }
    }

    private void MakeLabel(out Label created) {
        created = new Label();
        created.AddToClassList("list-text");
        container.Add(created);
    }

}
