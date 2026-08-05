using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SelectingController {
    
    private readonly PlatformController platformController;

    private readonly SelectingView view;
    private readonly SelectingModel model;

    public event Action OnSelectedPlatformChanged;

    public SelectingController(SelectingView view, PlatformController platformController) {
        this.view = view;
        this.platformController = platformController;
        model = new();
    }

    public int SelectedPlatformCount => model.SelectedPlatformIds.Count;
    public bool IsSelected(int platformId) => model.SelectedPlatformIds.Contains(platformId);

    public void Update() {
        ReadPlatformSelectionInput();
    }

    public void AddOption(int platformId) {
        model.OptionsPlatformIds.Add(platformId);
        view.AddPlatform(platformController.ReadPlatformState(platformId));
    }

    private void ReadPlatformSelectionInput() {
        var toggledIds = Enumerable.Empty<int>();

        if (ReadSelectAllPressed()) {
            bool partiallySelected = 
                model.SelectedPlatformIds.Count != model.OptionsPlatformIds.Count;
            
            toggledIds = partiallySelected
                ? model.OptionsPlatformIds.Except(model.SelectedPlatformIds)
                : model.OptionsPlatformIds;
        } else if (ReadSelectionIndexPressed(out var pressedIndex)) {
            toggledIds = new[] { model.OptionsPlatformIds[pressedIndex] };
        }

        bool hasEffect = false;
        foreach (var id in toggledIds) {
            hasEffect = true;
            if (!model.SelectedPlatformIds.Remove(id))
                model.SelectedPlatformIds.Add(id);
        }
        
        if (hasEffect) {
            OnSelectionChanged();
            OnSelectedPlatformChanged?.Invoke();
        }
    }

    private void OnSelectionChanged() {
        view.ShowNoPlatformSelected();
        if (SelectedPlatformCount != 0) {
            foreach (var selectedPlatformId in model.SelectedPlatformIds) {
                view.ShowPlatformSelected(platformController.ReadPlatformState(selectedPlatformId));
            }
        }
    }

    internal bool ReadSelectAllPressed() {
        return Input.GetKeyDown(KeyCode.Alpha0);
    }

    internal bool ReadSelectionIndexPressed(out int index) {
        var zeroIndexPressed = Input.GetKeyDown(KeyCode.Alpha1);
        var firstIndexPressed = Input.GetKeyDown(KeyCode.Alpha2);
        var secondIndexPressed = Input.GetKeyDown(KeyCode.Alpha3);
        index = -1;
        if (zeroIndexPressed) index = 0;
        else if (firstIndexPressed) index = 1;
        else if (secondIndexPressed) index = 2;
        return index >= 0;
    }

}