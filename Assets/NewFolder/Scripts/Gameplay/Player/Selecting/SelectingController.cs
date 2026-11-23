using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SelectingController {
    
    private readonly PlatformUI platformUI;
    private readonly TruckUI truckUI;
    private readonly AssemblingController assemblingController;

    private readonly SelectingView view;
    private readonly SelectingModel model;

    public SelectingController(SelectingView view, PlatformUI platformUI, 
        TruckUI truckUI, AssemblingController assemblingController) {
        this.view = view;
        this.platformUI = platformUI;
        this.truckUI = truckUI;
        this.assemblingController = assemblingController;
        model = new();
    }

    public IReadOnlyList<SelectionOption> Options => model.Options;
    public IReadOnlyList<SelectionOption> SelectedOptions => model.SelectedOptions;
    public bool HasSelection => model.SelectedOptions.Count > 0;

    public void Update() {
        SynchronizeOptions();
        ReadSelectionInput();
    }

    private void SynchronizeOptions() {
        var availableOptions = new List<SelectionOption>();
        foreach (var platformId in assemblingController.ControlledPlatformIds)
            availableOptions.Add(new SelectionOption(platformId, SelectionEntityType.Platform));
        if (assemblingController.HasTruckControlled)
            availableOptions.Add(SelectionOption.TruckOption);

        Diff.ComputeDiff(model.Options, availableOptions, out var addedOptions, out var removedOptions);
        foreach (var option in removedOptions) {
            model.Options.Remove(option);
            view.RemoveOption(option);
        }
        foreach (var option in addedOptions) {
            model.Options.Add(option);
            view.AddOption(option);
        }
        
        bool selectionChanged = addedOptions.Count > 0 || removedOptions.Count > 0;
        if (selectionChanged)
            OnSelectionChanged();
    }

    private void ReadSelectionInput() {
        var toggledOptions = Enumerable.Empty<SelectionOption>();

        bool hasEffect = false;
        if (ReadSelectionIndexPressed(out var pressedIndex)) {
            if (pressedIndex < model.Options.Count)
                toggledOptions = new[] { model.Options[pressedIndex] };
        }

        foreach (var option in toggledOptions) {
            hasEffect = true;
            ToggleSelected(option);
        }
        
        if (hasEffect) {
            OnSelectionChanged();
        }
    }

    private void OnSelectionChanged() {
        foreach (var option in model.Options) {
            if (option.entityType == SelectionEntityType.Platform) {
                platformUI.SetSelected(option.entityId, IsSelected(option));
            } else {
                truckUI.SetSelected(IsSelected(option));
            }
        }    

        view.ShowNoOptionSelected();
        foreach (var selectedOption in model.SelectedOptions)
            view.ShowOptionSelected(selectedOption);
    }

    private bool IsSelected(SelectionOption option) => model.SelectedOptions.Contains(option);

    private void ToggleSelected(SelectionOption option) {
        if (!model.SelectedOptions.Remove(option))
            model.SelectedOptions.Add(option);
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
