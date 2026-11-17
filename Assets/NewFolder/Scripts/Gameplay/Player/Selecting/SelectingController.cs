using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SelectingController {
    
    private readonly PlatformController platformController;
    private readonly TruckController truckController;
    private readonly PlatformUI platformUI;
    private readonly TruckUI truckUI;
    private readonly AssemblingController assemblingController;

    private readonly SelectingView view;
    private readonly SelectingModel model;

    public SelectingController(SelectingView view, PlatformController platformController, TruckController truckController,
        PlatformUI platformUI, TruckUI truckUI, AssemblingController assemblingController) {
        this.view = view;
        this.platformController = platformController;
        this.truckController = truckController;
        this.platformUI = platformUI;
        this.truckUI = truckUI;
        this.assemblingController = assemblingController;
        model = new();
    }

    public int SelectedPlatformCount => model.SelectedPlatformIds.Count;
    public IReadOnlyList<int> SelectedPlatformIds => model.SelectedPlatformIds;
    public bool IsTruckSelected => model.IsTruckSelected;
    public bool HasSelection => SelectedPlatformCount > 0 || model.IsTruckSelected;

    public void Update() {
        SynchronizeOptions();
        ReadPlatformSelectionInput();
    }

    private void SynchronizeOptions() {
        AssemblingController.ComputeIdDiff(model.OptionsPlatformIds, assemblingController.ControlledPlatformIds,
            out var addedIds, out var removedIds);
        bool selectionChanged = addedIds.Count > 0 || removedIds.Count > 0;
        foreach (var platformId in removedIds) {
            model.OptionsPlatformIds.Remove(platformId);
            selectionChanged |= model.SelectedPlatformIds.Remove(platformId);
            view.RemovePlatform(platformId);
        }
        foreach (var platformId in addedIds) {
            model.OptionsPlatformIds.Add(platformId);
            view.AddPlatform(platformController.ReadPlatformState(platformId));
        }
        if (!truckController.UnitExist && model.IsTruckSelected) {
            model.IsTruckSelected = false;
            selectionChanged = true;
        }
        if (selectionChanged)
            OnSelectionChanged();
    }

    private void ReadPlatformSelectionInput() {
        var toggledIds = Enumerable.Empty<int>();

        bool hasEffect = false;
        if (ReadSelectAllPressed()) {
            bool partiallySelected = model.SelectedPlatformIds.Count != model.OptionsPlatformIds.Count
                || (truckController.UnitExist && !model.IsTruckSelected);
            
            var selectedIds = partiallySelected
                ? model.OptionsPlatformIds.Except(model.SelectedPlatformIds).ToList()
                : model.OptionsPlatformIds.ToList();
            foreach (var id in selectedIds) {
                hasEffect = true;
                if (!model.SelectedPlatformIds.Remove(id))
                    model.SelectedPlatformIds.Add(id);
            }
            if (truckController.UnitExist) {
                model.IsTruckSelected = partiallySelected;
                hasEffect = true;
            }
        } else if (ReadSelectionIndexPressed(out var pressedIndex)) {
            if (pressedIndex < model.OptionsPlatformIds.Count)
                toggledIds = new[] { model.OptionsPlatformIds[pressedIndex] };
        } else if (ReadTruckSelectionPressed() && truckController.UnitExist) {
            model.IsTruckSelected = !model.IsTruckSelected;
            hasEffect = true;
        }

        foreach (var id in toggledIds) {
            hasEffect = true;
            if (!model.SelectedPlatformIds.Remove(id))
                model.SelectedPlatformIds.Add(id);
        }
        
        if (hasEffect) {
            OnSelectionChanged();
        }
    }

    private void OnSelectionChanged() {
        foreach (var platformId in model.OptionsPlatformIds)
            platformUI.SetSelected(platformId, model.SelectedPlatformIds.Contains(platformId));
        if (truckController.UnitExist)
            truckUI.SetSelected(truckController.ReadVehicleId().Value, model.IsTruckSelected);

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

    internal bool ReadTruckSelectionPressed() {
        return Input.GetKeyDown(KeyCode.T);
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
