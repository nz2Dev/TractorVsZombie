using System.Collections.Generic;

using UnityEngine.UIElements;

public class SelectingView {

    private readonly UIDocument uiDocument;

    private VisualElement container;
    private Dictionary<int, Label> binding = new();

    public SelectingView(UIDocument uiDocument) {
        this.uiDocument = uiDocument;
        container = uiDocument.rootVisualElement.Q<VisualElement>("platformList");
        container.Clear();
    }

    internal void AddPlatform(PlatformState state) {
        MakeLabel(out var label);
        binding[state.platformId] = label;
        UpdateLabel(label, state);
    }

    internal void UpdatePlatform(PlatformState state) {
        var stateLabel = binding[state.platformId];
        UpdateLabel(stateLabel, state);
    }

    internal void ShowPlatformSelected(PlatformState selectedPlatformState) {
        foreach (var platformId in binding.Keys) {
            var isSelectedKey = selectedPlatformState.platformId == platformId;
            if (isSelectedKey) {
                var label = binding[platformId];
                label.AddToClassList("selected-label");
            }
        }
    }

    internal void ShowNoPlatformSelected() {
        foreach (var label in binding.Values) {
            label.RemoveFromClassList("selected-label");
        }
    }

    private void MakeLabel(out Label created) {
        created = new Label();
        container.Add(created);
    }

    private void UpdateLabel(Label label, PlatformState state) {
        label.text = $"weapon {state.weaponId}";
    }
}