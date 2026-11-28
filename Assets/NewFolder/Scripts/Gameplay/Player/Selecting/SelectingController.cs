using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SelectingController {
    
    private readonly PlatformUI platformUI;
    private readonly TruckUI truckUI;
    private readonly AssemblingController assemblingController;
    private readonly PlatformController platformController;
    private readonly TruckController truckController;

    private readonly SelectingView view;
    private readonly SelectingModel model;
    private SelectionPrototype prototype;

    public SelectingController(SelectingView view, PlatformUI platformUI,
        TruckUI truckUI, AssemblingController assemblingController, PlatformController platformController, TruckController truckController) {
        this.view = view;
        this.platformUI = platformUI;
        this.truckUI = truckUI;
        this.assemblingController = assemblingController;
        this.platformController = platformController;
        this.truckController = truckController;
        model = new();
    }

    public IReadOnlyList<SelectionOption> Options => model.Options;
    public IReadOnlyList<SelectionOption> SelectedOptions => model.SelectedOptions;
    public bool HasSelection => model.SelectedOptions.Count > 0;

    public void Init(SelectionPrototype prototype) {
        this.prototype = prototype;
    }

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
        model.Options.Clear();
        model.Options.AddRange(availableOptions);

        var availableCategories = new HashSet<WeaponCategory>();
        var optionCategoriesCount = 0;
        foreach (var item in availableOptions) {
            var category = GetOptionCategory(item);
            if (category != null) {
                availableCategories.Add(category);
                optionCategoriesCount++;
            }
        }

        bool optionCategoriesChanged = optionCategoriesCount != model.OptionsWithCategoryCount;
        model.OptionsWithCategoryCount = optionCategoriesCount;
        
        Diff.ComputeDiff(model.Categories, availableCategories, out var addedCategories, out var removedCategories);
        foreach (var category in removedCategories) {
            model.Categories.Remove(category);
            view.RemoveCategory(category);
        }
        foreach (var category in addedCategories) {
            model.Categories.Add(category);
            view.AddCategory(category, prototype.listItemTreeAsset, category.weaponImage);
        }
        
        model.SelectedCategories.RemoveAll(category => !model.Categories.Contains(category));
        foreach (var category in model.Categories) {
            view.UpdateCategoryOptionsCount(category, CountCategoryOptions(category));
        }

        if (model.Categories.Count == 0) {
            model.SelectedCategoryState = 0;
        } else if (model.SelectedCategoryState > model.Categories.Count) {
            model.SelectedCategoryState = model.Categories.Count;
        }
        
        if (addedCategories.Count > 0 || removedCategories.Count > 0 || 
            addedOptions.Count > 0 || removedOptions.Count > 0 || optionCategoriesChanged) {
            OnSelectionChanged();
        }
    }

    private void ReadSelectionInput() {
        if (ReadMouseWheelDelta(out var wheelDelta)) {
            var direction = wheelDelta > 0 ? 1 : -1;
            var statesCount = model.Categories.Count + 1;
            model.SelectedCategoryState =
                (model.SelectedCategoryState + direction + statesCount)
                % statesCount;

            model.SelectedCategories.Clear();
            if (model.SelectedCategoryState > 0) {
                var selectedIndex = model.SelectedCategoryState - 1;
                model.SelectedCategories.Add(model.Categories[selectedIndex]);
            }

            OnSelectionChanged();
        }

        if (ReadSelectionIndexPressed(out var pressedIndex)) {
            if (pressedIndex >= model.Categories.Count)
                return;

            var pressedCategory = model.Categories[pressedIndex];
            if (!model.SelectedCategories.Remove(pressedCategory)) {
                model.SelectedCategories.Add(pressedCategory);
                model.SelectedCategoryState = pressedIndex;
            }
            
            OnSelectionChanged();
        }
    }

    private void OnSelectionChanged() {
        model.SelectedOptions.Clear();
        foreach (var option in model.Options) {
            var optionCategory = GetOptionCategory(option);
            if (model.SelectedCategories.Contains(optionCategory)) {
                model.SelectedOptions.Add(option);
            }
        }

        foreach (var option in model.Options) {
            if (option.entityType == SelectionEntityType.Platform) {
                if (IsSelected(option)) {
                    platformUI.ShowSelectedOutline(option.entityId);
                } else {
                    platformUI.ShowNoOutline(option.entityId);
                    var optionCategory = GetOptionCategory(option);
                }
            } else {
                if (IsSelected(option)) {
                    truckUI.ShowSelectedOutline();
                } else {
                    truckUI.ShowNoOutline();
                }
            }
        }    

        view.ShowNoOptionSelected();
        foreach (var category in model.SelectedCategories) {
            view.ShowCategorySelected(category);
        }
    }

    private int CountCategoryOptions(WeaponCategory category) {
        return model.Options.Count(option => GetOptionCategory(option) == category);
    }

    private bool IsSelected(SelectionOption option) 
        => model.SelectedOptions.Contains(option);

    private WeaponCategory GetOptionCategory(SelectionOption option) {
        if (option.entityType == SelectionEntityType.Platform) {
            var platformState = platformController.ReadPlatformState(option.entityId);
            if (!platformState.hasLoadout) {
                return null;
            }
            return platformState.loadoutState.weaponState.category;
        } else {
            var truckState = truckController.ReadState();
            return truckState.attachmentState.weaponState.category;
        }
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

    private bool ReadMouseWheelDelta(out float delta) {
        delta = Input.mouseScrollDelta.y;
        return delta != 0;
    }

}
