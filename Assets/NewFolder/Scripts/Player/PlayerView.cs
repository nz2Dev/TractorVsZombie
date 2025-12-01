using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class PlayerView {

    private readonly UIDocument uiDocument;

    private VisualElement container;
    private Dictionary<TowableVehicleId, Label> binding = new();

    public PlayerView(UIDocument uiDocument) {
        this.uiDocument = uiDocument;
        container = uiDocument.rootVisualElement.Q<VisualElement>("platformList");
        container.Clear();
    }

    internal void AddPlatform(PlatformState state) {
        MakeLabel(out var label);
        binding[state.vehicleId] = label;
        UpdateLabel(label, state);
    }

    internal void UpdatePlatform(PlatformState state) {
        var stateLabel = binding[state.vehicleId];
        UpdateLabel(stateLabel, state);
    }

    internal void UpdateSelectedPlatform(PlatformState selectedPlatformState) {
        foreach (var vehicleId in binding.Keys) {
            var isSelectedKey = selectedPlatformState.vehicleId == vehicleId;
            var label = binding[vehicleId];
            label.RemoveFromClassList("selected-label");
            if (isSelectedKey) {
                label.AddToClassList("selected-label");
            }
        }
    }

    private void MakeLabel(out Label created) {
        created = new Label();
        container.Add(created);
    }

    private void UpdateLabel(Label label, PlatformState state) {
        label.text = state.weaponConfig == null ? "empty" : state.weaponConfig.visualsPrefab.name;
    }

}