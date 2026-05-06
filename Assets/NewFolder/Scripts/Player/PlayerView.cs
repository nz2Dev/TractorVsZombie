using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class PlayerView {

    private readonly UIDocument uiDocument;
    private readonly CameraManager cameraManager;

    private VisualElement container;
    private Dictionary<int, Label> binding = new();
    private AimVisuals aimVisuals;

    public PlayerView(UIDocument uiDocument, CameraManager cameraManager) {
        this.uiDocument = uiDocument;
        container = uiDocument.rootVisualElement.Q<VisualElement>("platformList");
        container.Clear();
        this.cameraManager = cameraManager;
    }

    internal void SetAimVisuals(AimVisuals aimVisualsPrefab) {
        aimVisuals = GameObject.Instantiate(aimVisualsPrefab);
        aimVisuals.HideSelf();
    }

    internal void ShowAim(TopDownAimInput aimInput) {
        aimVisuals.ShowSelf();
        aimVisuals.Transform(aimInput);
    }

    internal void UpdateAim(TopDownAimInput aimInput) {
        aimVisuals.Transform(aimInput);
    }

    internal void HideAim() {
        aimVisuals.HideSelf();
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

    internal void EnableFollowCamera(Vector3 position) {
        cameraManager.InitTopDownFollowTarget(position);
    }

    internal void UpdateFollowCamera(Vector3 position) {
        cameraManager.UpdateTopDownFollowPosition(position);
    }

}