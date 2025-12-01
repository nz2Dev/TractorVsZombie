using System;
using System.Collections.Generic;

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

    private void MakeLabel(out Label created) {
        created = new Label();
        container.Add(created);
    }

    private void UpdateLabel(Label label, PlatformState state) {
        var weaponFirstWord = FirstWord(state.weaponConfig.visualsPrefab.name);
        label.text = state.weaponConfig == null ? "empty" : weaponFirstWord;
    }

    private string FirstWord(string line) {
        return line.Split(' ')[0];
    }

}